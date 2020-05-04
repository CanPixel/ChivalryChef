using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIKnight : KnightMovement {
	public PlayerSettings playerSettings;

	//AI states, some are unused (for now)
	public enum State {
		REST, SCAN, MOVE, ATTACK, BRAWL
	}
	[HideInInspector]
	public State state = State.REST;

	public bool scanActively = true;

	public GameObject currentTarget;
	[HideInInspector]
	public GameObject lastTarget;
	[HideInInspector]
	public float currentDist;

	public bool includePlayersAsTarget;
	[HideInInspector]
	public List<KnightMovement> targets = new List<KnightMovement>();

	//This is the range that triggers a change in AI state to following certain targets 
	public Vector2 followRange = new Vector2(5f, 15f);

	private float lifetime;

	private int timesAttacked = 0;
	private bool brawling = false;
	private float brawlDistance = 0;
	private float brawlDelay = 0;

	private bool forceMove = false;

	void OnValidate() {
		if(followRange.y < followRange.x) {
			var follow = followRange.y;
			followRange.y = followRange.x;
			followRange.x = follow;
		}
		if(followRange.x < 0) followRange.x = 0;
		if(followRange.y < 0) followRange.y = 0;
	}

	protected override void Start() {
		base.Start();
		state = State.REST;
		attackDelay = 1f;
	}

	void Update () {
		ApplyGravity();
		ApplyMovement();
		
		if(ShouldThink()) {
			Think();
		}
		ExecuteAI();
		Move(true);

		Animate();

		lifetime += Time.deltaTime;
	}

	protected bool ShouldThink() {
		return !forceMove;
	}

	protected void Think() {
		//Priorities & other stuff later here
		if(lifetime > 1 && (currentTarget == null) && scanActively) ScanForTarget();

		if(currentTarget != null) {
			currentDist = Vector3.Distance(transform.position, currentTarget.transform.position);
			if(currentDist > followRange.x && currentDist < followRange.y && !brawling) SetState(State.MOVE);
			else SetState(State.ATTACK);
		
			if(brawling) SetState(State.BRAWL);
			else {
				if(lastTarget != currentTarget) timesAttacked = 0;
				//SetState(State.ATTACK);
				lastTarget = currentTarget;
			}
		}
		
		if(currentDist >= followRange.y && currentTarget == null && !brawling) SetState(State.REST);
	}

	protected void ExecuteAI() {
		if(brawlDelay > 0) brawlDelay -= Time.deltaTime;

		if(currentTarget == null) SetState(State.REST);

		switch(state) {
			default:
			case State.REST:
				desiredMove = targetMove = Vector3.zero;
				break;
			case State.ATTACK:
				if(currentDist > followRange.y) SetState(State.MOVE);
				else {
					if(!brawling) AttackTarget();
					else SetState(State.BRAWL);
				}
				break;
			case State.BRAWL:
				if(brawlDelay <= 0) {
					SetState(State.ATTACK);
					brawling = false;
					break;
				}
				mouseAttack = false;
				brawling = true;
				MoveToTarget(currentTarget.transform.position - transform.forward * -brawlDistance);
				break;
			case State.MOVE:
				mouseAttack = brawling = false;
				if(currentTarget != null) {
					if(!forceMove) MoveToTarget(currentTarget.transform.position);
					else ForceMoveToTarget();
				}
				break;
		}
	}

	protected void AttackTarget() {
		if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) OnAttackFinish();

		if(currentDist >= followRange.y) {
			SetState(State.MOVE);
			return;
		}

		if(currentDist < followRange.x && !attacking && attackTick <= 0) {
			mouseAttack = true;
			attackTick = attackDelay;
		} else mouseAttack = false;

		attacking = mouseAttack;
	}

	public void ForceAttack() {
		attacking = true;
	}

	protected override void OnAttackFinish() {
		base.OnAttackFinish();
		timesAttacked++;
		brawlDistance = Random.Range(10, 20);
		brawlDelay = Random.Range(1f, 3f);
		brawling = true;
		SetState(State.BRAWL);
		if(Player.host != null) CineCircleManager.NotifyCinematic(10f, CineCircle.ATTACK, gameObject, currentTarget);
	}

	public void ForceToTarget(GameObject target) {
		forceMove = true;
		currentTarget = target;
		SetState(State.MOVE);
	}

	protected void ForceMoveToTarget() {
		var dir = (currentTarget.transform.position - transform.position).normalized;
		move = new Vector3(0, move.y, -(speed / 15f));
		var rot = Quaternion.LookRotation(dir).eulerAngles.y + 180;
		chivalryKnight.transform.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(chivalryKnight.transform.localEulerAngles.y, rot, Time.deltaTime * (speed / 10f)), 0);
	}

	protected void MoveToTarget(Vector3 pos) {
		if(currentDist < followRange.x && !brawling) return;
		var dir = (pos - transform.position).normalized;
		move = new Vector3(dir.x, move.y, dir.z);
		chivalryKnight.transform.LookAt(currentTarget.transform, Vector3.up);
	}

	protected void ScanForTarget() {
		targets.Clear();

		//[[Optimization needed]]
		foreach(var player in GameObject.FindGameObjectsWithTag("Knight")) {
			var comp = player.GetComponent<KnightMovement>();
			if(comp == null) continue;
			if(comp.GetType() != typeof(Player) || (includePlayersAsTarget && comp.GetType() == typeof(Player))) targets.Add(comp);
		}

		targets = targets.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
		
		if(targets.Count > 1) {
			for(int i = 1; i < targets.Count; i++) if(targets[i].gameObject != null) {
				currentTarget = targets[i].gameObject;
				break;
			}
		}
	}

	protected void SetState(State state) {
		this.state = state;
	}

	protected void Animate() {
		base.Animate(playerSettings);
	}

	protected override void Move(bool rotate = true) {
		if(rotate) transform.Rotate(move.z * Time.deltaTime, move.x * Time.deltaTime, 0);
		controller.Move((transform.forward * move.z * speed) * Time.deltaTime);
		controller.Move((transform.right * move.x * speed * 0.5f) * Time.deltaTime);
		controller.Move(new Vector3(0, move.y, 0));
	}

	void OnGUI() {
		//does this work in a devbuild?
		if(Player.DEV_VIEW) {
			var screen = Camera.main.WorldToScreenPoint(transform.position);
			if(screen.z < 0) return;

			var oldCol = GUI.color;
			GUI.color = Color.black;
			var statePos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 4);
			GUI.Label(new Rect(statePos.x, Screen.height - statePos.y, Screen.width * 10, Screen.height * 10), state.ToString());

			if(currentTarget != null) {
				GUI.color = Color.red;
				var targetPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 4.5f);
				var screenPos = Screen.height - targetPos.y;
				GUI.Label(new Rect(targetPos.x, screenPos, Screen.width * 10, Screen.height * 10), currentTarget.gameObject.name.ToString());
			}

			GUI.color = oldCol;
		}
	}
}
