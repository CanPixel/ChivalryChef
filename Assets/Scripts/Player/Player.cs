using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class Player : KnightMovement {
	#region REFERENCES

	[Space(10)]
	public GameObject[] playerCanvasElements;
	[Space(10)]
	public GameObject cineCirclePrefab;
	public GameObject slashTrailPrefab;
	public RawImage hurtOverlay;
	public GameObject chivalryAxe;

	public Text deathText;
	public Tutorial sprint;

	#endregion

	//Individual player settings
	public GameSettings playerPrefs;

	//Cap on amount of attacks 
	public const float DelayInBetweenAttacks = 0.2f;

	//Toggles debug mode
	public static bool DEV_VIEW = false;
	//Is the inventory open?
	public static bool IsInventory = false;

	private float pickDelay = 0;
	public SmoothMouseLook mouseLook;

	public static Player host;

	//Camera variables
	private float zoom = 0;
	private float baseZoom;
	public float zoomIntensity = 3;
	private float zoomMagnify = 1;

	private float timeUntilSprint = 0;

	//Reference to UI element for picking up items
	public PickupLabel label;
	private float pickupDistance = -1;
	protected GameObject currentPickingItem;

	public bool isZoom {
		get {return Input.GetMouseButton(1);}
	}
	public override bool isSprinting {
		get {return Input.GetKey(KeyCode.LeftShift) && stamina > 0 && targetMove.z > 0;}
	}
	protected override bool isJumping() {
		return controller != null && controller.isGrounded && Input.GetButtonDown("Jump");
	}

	private List<Image> hurtOverlays = new List<Image>();

	private float attackDistinguisher = 0;

	#region HEAVY_ATTACK
	Vector2 mouseStart, mouseEnd, mouseDiff, mouseDest;
	#endregion

	protected override void Start() {
		host = this;
		base.Start();
		baseZoom = Camera.main.fieldOfView;
		hurtOverlays = GameObject.FindGameObjectsWithTag("Hurt").Select(x => x.GetComponent<Image>()).ToList();
		SetHurtOverlayAlpha(0);
		OnPlayerSettingChanged();
	}

	protected override void Move(bool rotate = true) {
		if(rotate) transform.Rotate(move.z * Time.deltaTime, move.x * Time.deltaTime, 0);
		if(controller == null) return;
		if(GameMenu.MenuOn) move.x = move.z = 0;
		controller.Move((transform.forward * move.z * speed * (isSprinting ? sprintBoost : 1) * (isJumping() ? 1.5f : 1)) * Time.deltaTime);
		controller.Move((transform.right * move.x * speed * 0.5f) * Time.deltaTime);
		controller.Move(new Vector3(0, move.y, 0));
	}

	void Update () {
		InteractWithGrass();

		if(!sprint.finished) timeUntilSprint += Time.deltaTime;
		if(pickDelay > 0) pickDelay -= Time.deltaTime;
		
		FetchInputs();
		ApplyGravity();
		ApplyMovement();
		Move(isMoving());
		Animate();

		if(hurtTick > 0) SetHurtOverlayAlpha((hurtDelay / hurtTick) - 1f);
	}

	protected void InteractWithGrass() {
		Shader.SetGlobalVector("_Obstacle", transform.position);
	}

	private void SetHurtOverlayAlpha(float a) {
		if(hurtOverlays == null || hurtOverlays.Count <= 0) return;
		
		for(int i = 0; i < hurtOverlays.Count; i++) hurtOverlays[i].color = new Color(1, 1, 1, a);
		hurtOverlay.color = new Color(hurtOverlay.color.r, hurtOverlay.color.g, hurtOverlay.color.b, Mathf.Clamp(a, 0, 0.8f));
	}

	#region SWING CAMERA FX
	private float swingZoom;
	public void TriggerSwingZoom(float amount = 0, float mag = 1) {
		if(amount == 0) swingZoom = 1;
		else swingZoom = amount;
		zoomMagnify = mag;
	}
	public void CancelSwing() {
		swingZoom = 0;
		zoomMagnify = 1;
	}
	#endregion

	protected void Animate() {
		base.Animate(playerPrefs.playerSettings);
	}

	protected void FetchInputs() {
		if(Input.GetKeyUp(KeyCode.F5)) DEV_VIEW = !DEV_VIEW;

		if(isSprinting) {
			if(sprint != null && !sprint.finished) {
				TutorialManager.FinishTutorial(sprint);
				sprint.finished = true;
			}
			sprintTime += Time.deltaTime;
			DrainStamina(Time.deltaTime);
		} else sprintTime -= Time.deltaTime;

		if(sprintTime <= 0 && !isSprinting) {
			ChargeStamina(0.05f);
			sprintTime = 0;
		}
		if(!sprint.finished && lifeTime > 3) TutorialManager.SetTutorial(sprint, gameObject.transform.position + new Vector3(250, -100 , 0));

		if(!IsAnimation("Swing")) mouseLook.freeze = IsInventory | GameMenu.MenuOn;
		if(GameMenu.MenuOn) return;
		if(controller != null && controller.isGrounded) targetMove = new Vector3(Input.GetAxis("Horizontal"), targetMove.y, Input.GetAxisRaw("Vertical"));
		var mouse = (isZoom ? 1 : 0) * -(zoomIntensity * 10f);
		if(IsInventory) mouse = 0;
		if(swingZoom != 0) zoom = Mathf.Lerp(zoom, (zoomIntensity * 15f) * zoomMagnify, Time.deltaTime * 4f);
		else zoom = Mathf.Lerp(zoom, mouse * zoomMagnify, Time.deltaTime * 4);
		Camera.main.fieldOfView = baseZoom + zoom;
		
		//MELEE ATTACKING / COMBAT 
		#region COMBAT_LOGIC
		if(attackDelay > 0) attackDelay -= Time.deltaTime;
		float distinguishDelay = 0.125f;
		if(IsInventory) return;
		mouseAttack = Input.GetButtonDown("Attack");
		if(Input.GetButtonDown("Attack") && attackDelay <= 0) {
			mouseStart = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			attackDistinguisher = Time.time;
		}
		if((Time.time - attackDistinguisher) < distinguishDelay) { //Fast Attack
			if(Input.GetButtonUp("Attack") && attackDelay <= 0) {
				attackDelay = DelayInBetweenAttacks;
				mouseEnd = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mouseDiff = mouseEnd - mouseStart;
				PerformFastAttack();
			}
			return;
		}
		if((Time.time - attackDistinguisher) >= distinguishDelay) { //Strong Attack
			if(Input.GetButtonUp("Attack") && attackDelay <= 0) {
				attackDelay = DelayInBetweenAttacks;
				mouseEnd = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mouseDiff = mouseEnd - mouseStart;
				if(Mathf.Abs(mouseDiff.x) + Mathf.Abs(mouseDiff.y) <= 0.2f) {
					PerformFastAttack();
					return;
				}
				PerformStrongAttack();
			} else if(attackDistinguisher != 0) {
				mouseAttack = attacking = false;
				mouseLook.freeze = true;
				Cursor.lockState = CursorLockMode.Locked;
            	Cursor.lockState = CursorLockMode.None;
			}
		}
		#endregion 
	}

	protected void PerformStrongAttack() {
		mouseLook.freeze = true;
		attackDistinguisher = 0;
		mouseDest = mouseDiff * -1f;
		GenerateSlashTrail(true);
		if(mouseDest.x < 0) AttackSwingRight();
		else AttackSwingLeft();
		Cursor.lockState = CursorLockMode.Locked;
		SoundManager.PLAY_UNIQUE_SOUND("Swordswing", 1.2f, 0.25f, 0.5f);
	}
	protected void PerformFastAttack() {
		attackDistinguisher = 0;
		attacking = true;
		GenerateSlashTrail(false);
		SoundManager.PLAY_UNIQUE_SOUND("Swordhit", 0.6f, 0.1f, 1.03f);
	}

	protected void GenerateSlashTrail(bool heavy) {
		var obj = Instantiate(slashTrailPrefab);
		obj.transform.SetParent(chivalryAxe.transform);
		obj.transform.localPosition = new Vector3(5, 0, 0);
	}

	public override void ResetItemPickup() {
		label.HideLabel();
		pickupDistance = -1;
		base.ResetItemPickup();
		currentPickingItem = null;
	}
	public override void NotifyPickup(Ingredient ing, GameObject part) {
		if(IsInventory) return;
		currentPickingItem = part;
		base.NotifyPickup(ing, part);
		label.SetLabel(ing.values, part, (health < maxHealth));
	}
	public override void NotifyPlayerLoot(KnightHead ing) {
		if(IsInventory) return;
		base.NotifyPlayerLoot(ing);
		currentPickingItem = ing.gameObject;
		label.SetLabel(ing, ing.gameObject);
	}
	protected override bool isPickingItem() {
		return Input.GetKeyDown(KeyCode.E) && pickDelay <= 0;
	}
	protected bool isEatingItem() {
		return Input.GetKeyDown(KeyCode.R) && pickDelay <= 0;
	}
	//Picking up ingredients
	public override void PickupItem(Ingredient ing, GameObject obj) {
		if(IsInventory) return;
		label.HideLabel();
		pickDelay = 0.3f;
		pickupDistance = -1;
		currentPickingItem = null;
		base.PickupItem(ing, obj);
	}

	//Eating of ingredients
	public void ConsumeItem(Ingredient ing, GameObject obj) {
		if(IsInventory) return;
		label.HideLabel();
		pickDelay = 0.3f;
		pickupDistance = -1;
		currentPickingItem = null;
		Heal(10);
		if(client != null) client.SendDamageToServer(client.playerIndex, health);

		var rigids = ing.GetComponentsInChildren<Rigidbody>();
		foreach(var rigid in rigids) Destroy(rigid.gameObject);
		SoundManager.PLAY_SOUND("takeItem", 0.7f, 1);
		SoundManager.PLAY_SOUND("crunch", 0.15f, 0.85f);

		var parts = Instantiate(Resources.Load("CRUNCH") as GameObject);
		parts.GetComponent<ParticleSystemRenderer>().material.color = ing.values.cutColor;
		parts.transform.position = obj.transform.position;
	}

	public override bool Hurt(int i, GameObject source, bool particles = true) {
		if(hurtTick <= hurtDelay) return false;
		mouseLook.HurtShake(new Vector3(0, (Random.Range(0, 4) <= 2) ? 10 : -10, (Random.Range(0, 4) <= 2) ? 20 : -20));
		SoundManager.PLAY_SOUND("Owww", 0.5f, Random.Range(0.9f, 1.1f));
		var booli = base.Hurt(i, source, particles);
		if(client != null) client.SendDamageToServer(client.playerIndex, health);
		return booli;
	}

	public override void SetHealth(float health) {
		if(hurtTick <= hurtDelay) return;
		mouseLook.HurtShake(new Vector3(0, (Random.Range(0, 4) <= 2) ? 10 : -10, (Random.Range(0, 4) <= 2) ? 20 : -20));
		SoundManager.PLAY_SOUND("Owww", 0.5f, Random.Range(0.9f, 1.1f));
		base.SetHealth(health);
		//if(client != null) client.SendDamageToServer(client.playerIndex, health);
	}

	public void OnPlayerSettingChanged() {
		mouseLook.UpdateCameraType();
		if(Camera.main.GetComponent<ImageFX>() != null) Camera.main.GetComponent<ImageFX>().enabled = playerPrefs.playerSettings.paintingShader;
		mouseLook.smoothing = playerPrefs.playerSettings.smoothing;
	}

	public override void Die() {
		SoundManager.SetOSTState(SoundManager.OSTEvent.DEATH);
		foreach(var i in playerCanvasElements) i.SetActive(false);
		var headBone = chivalryHeadBone.transform;
		SpawnSeasoningParticles();
		var head = Instantiate(chivalryHead, headBone.position, Quaternion.Euler(headBone.eulerAngles.x - 90, headBone.eulerAngles.y, headBone.eulerAngles.z));
		Camera.main.GetComponent<CameraDeathSequence>().StartSequence(head, lastHurtSource, this);
		SoundManager.PLAY_SOUND("Triumph", 1, 0.3f);
		Destroy(gameObject);
	}

	public override void TriggerBattle(KnightMovement src) {
		if(enemy == null || enemy.name != src.name) SoundManager.PLAY_SOUND("SUDDEN", 0.9f, 0.9f);
		base.TriggerBattle(src);
		SoundManager.SetOSTState(SoundManager.OSTEvent.BATTLE, true);
		TriggerSwingZoom(10);
		TutorialManager.SetTutorial("BattleSwing");
	}
	public override void UntriggerBattle(bool victory = false) {
		base.UntriggerBattle();
		SoundManager.SetOSTState(SoundManager.OSTEvent.GATHERING);
		SoundManager.SetCrescendo(0.5f);
		if(victory) SoundManager.PLAY_UNIQUE_SOUND("Triumph", 1f, 0, 0);
	}

	protected override void OnAttackFinish() {
		base.OnAttackFinish();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.lockState = CursorLockMode.None;
	}

	protected void OnTriggerStay(Collider col) {
		if(col.tag == "Food") {
			var ing = col.gameObject.GetComponentInParent<Ingredient>();
			float currentDist = Vector3.Distance(transform.position, col.transform.position);
			if(ing.owner == null && (pickupDistance > currentDist || pickupDistance < 0)) {
				TutorialManager.SetTutorial("BattleHack");
				NotifyPickup(ing, col.gameObject);
				pickupDistance = currentDist;
			}
			if(isPickingItem() && currentPickingItem == col.gameObject) PickupItem(ing, currentPickingItem);
			if(isEatingItem() && currentPickingItem == col.gameObject && health < maxHealth) ConsumeItem(ing, currentPickingItem);
		} else if(col.tag == "PlayerHead") {
			var head = col.gameObject.GetComponent<KnightHead>();
			if(head != null) {
				NotifyPlayerLoot(head);
				//
			}
		}
	}
	void OnTriggerExit(Collider col) {
		if(col.tag == "Food" || col.tag == "PlayerHead") {
			if(col.gameObject == label.GetObject()) ResetItemPickup();
		}
	}
}

public abstract class KnightMovement : MonoBehaviour {
	[HideInInspector]
	public Client client;

	#region Variables
	public float health = 100;
	public float maxHealth = 100;
	public float stamina = 100;
	public float maxStamina = 100;
	public float staminaDrainFactor = 2f;

	public Transform neck;

	protected float lifeTime = 0;
	
	public float knightDMG = 10f;
	public float knightSwingBonusFactor = 1.5f;

	protected GameObject lastHurtSource;

	protected const float hurtDelay = 5;
	[SerializeField]
	protected bool inBattle = false;
	protected KnightMovement enemy;

	public float sprintBoost = 1.5f;
	protected float sprintTime;

	public class ItemPickup {
		public GameObject pickedItem;
		public float pickedItemCurve;
		public Quaternion targetRot;
		public Vector3 targetOffs;

		public bool gettingPickedUp = true;

		public int parts;

		public ItemPickup(GameObject item, GameObject backpack) {
			pickedItem = item;
			pickedItemCurve = 0;
			targetRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
			targetOffs = new Vector3(Random.Range(-0.4f, 0.4f), 0, Random.Range(-0.3f, 0.4f));
			foreach(Transform t in pickedItem.transform) if(t.GetComponent<MeshCollider>() != null) {
				t.GetComponent<MeshCollider>().enabled = false;
				t.localPosition = Vector3.zero;
			}
			pickedItem.transform.SetParent(backpack.transform);
			parts = pickedItem.GetComponentsInChildren<Rigidbody>().Length - 1;
			if(parts > 0) foreach(Transform i in pickedItem.transform) i.localPosition = new Vector3(0, 0, Random.Range(-1f, 0f));
		}
	}
	protected List<ItemPickup> pickedItems = new List<ItemPickup>();

	public virtual bool isSprinting {
		get {return sprintTime > 0;}
	}

	protected bool mouseAttack = false;

	[System.Serializable]
	public class PlayerSettings {
		[Range(0.1f, 1f)]
		public float xSensitivity = 0.3f, ySensitivity = 0.1f;
		[Range(0, 50)]
		public float smoothing = 12f;

		[Range(0, 1)]
		public float soundVol = 1, musicVol = 1, ambientVol = 1;

		public bool showMarkers = true;
		public bool paintingShader = true;

		[SerializeField]
		public bool staticCamera = false;
	}

	public GameObject chivalryKnight, chivalryHead, chivalryHeadBone;
	public GameObject backpack;
	public GameObject seasoningParticlesPrefab;

	protected Inventory inventory;
	public Inventory GetInventory() {
		return inventory;
	}

	public float speed = 6.0f;

	protected Animator animator;
	[HideInInspector]
	public CharacterController controller;

	protected Vector3 desiredMove, move, targetMove;
	public float jumpHeight = 1.5f;
	public float gravity = 20.0f;

	protected bool attacking = false;
	protected float attackTick = 0, attackDelay = 0;

	[HideInInspector]
	public int swingDirection = 1;
	[System.Serializable]
	public enum MELEE_ACTION {
		NONE, HACK, SWING
	}
	[HideInInspector]
	public MELEE_ACTION meleeAction;

	protected float hurtBounceAmp = 0, hurtBounceSpeed = 0, hurtTick = 0;
	protected Vector3 knightBaseScale;

	protected ParticleSystem boilingParticles;
	#endregion

	protected float neckRot;
	protected bool updateNeck = false;

	public void SetNeckRotation(float i) {
		if(neck != null && neck.gameObject != null) {
			updateNeck = true;
        	neckRot = i;
		}
	}

	public bool IsItemGettingPickedUp(int count) {
		return pickedItems[count] != null && pickedItems[count].gettingPickedUp;
	}

	public Vector3 GetTargetMove() {
		return targetMove;
	}

	public void MoveTo(Vector3 pos) {
		//if(currentDist < followRange.x && !brawling) return;
		//var dir = (pos - transform.position).normalized;
		targetMove = new Vector3(pos.x, targetMove.y, pos.z);
		//chivalryKnight.transform.LookAt(currentTarget.transform, Vector3.up);
	}

	protected bool isMoving() {
		return move.x != 0 || move.z != 0;
	}
	protected virtual bool isPickingItem() {
		return false;
	}
	protected virtual bool isJumping() {
		return false;
	}

	protected virtual void Start () {
		animator = chivalryKnight.GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		stamina = maxStamina;
		inventory = GetComponent<Inventory>();
		knightBaseScale = animator.transform.localScale;
		boilingParticles = Util.FindChildByTag(transform, "BoilingParticles").GetComponent<ParticleSystem>(); 
		ToggleBoilParticles(false);
		client = GetComponent<Client>();
	}

	public void ToggleBoilParticles(bool i) {
		if(i) boilingParticles.Play();
		else boilingParticles.Stop();
	}

	protected void LateUpdate() {
		if(updateNeck) neck.rotation *= Quaternion.AngleAxis(neckRot, new Vector3(1, 0, 0));

		lifeTime += Time.deltaTime;
		if(hurtTick <= hurtDelay) ApplyHurtBounce();

		//Battle OST fade
		if(enemy != null) {
			var dist = Vector3.Distance(transform.position, enemy.transform.position);
			if(dist > 100) UntriggerBattle();
		} else if(inBattle) UntriggerBattle(true);
	}

	protected void ApplyGravity() { 
		if(isJumping()) {
			targetMove.y = jumpHeight * (isSprinting ? 1.05f : 0.85f);
			SoundManager.PLAY_SOUND("Leap", 0.4f, 1.5f);
		}
		targetMove.y -= gravity * Time.deltaTime;
	}
	protected virtual void Move(bool rotate = true) {
		if(rotate) transform.Rotate(move.z * Time.deltaTime, move.x * Time.deltaTime, 0);
		if(controller == null) return;
		controller.Move((transform.forward * move.z * speed * (isSprinting ? sprintBoost : 1) * (isJumping() ? 1.5f : 1)) * Time.deltaTime);
		controller.Move((transform.right * move.x * speed * 0.5f) * Time.deltaTime);
		controller.Move(new Vector3(0, move.y, 0));
	}
	protected void ApplyMovement() {
		desiredMove = new Vector3(targetMove.x, targetMove.y, targetMove.z);
		move = desiredMove;
	}

	protected void Animate(PlayerSettings playerSettings) {
		if(!controller.isGrounded && !animator.GetBool("isJumping")) {
			animator.SetTrigger("Jump");
			animator.SetBool("isJumping", true);
		} if(controller.isGrounded) animator.SetBool("isJumping", false);

		animator.SetBool("shouldRun", isMoving());
		animator.SetFloat("warmupRun", isMoving() ? (move.z * speed / 10) * (isSprinting ? sprintBoost - 0.2f : 1) : 1);
		if(move.z == 0) animator.SetFloat("warmupRun", isMoving() ? (move.x * playerSettings.xSensitivity) : 1);
		if(attacking) {
			animator.SetTrigger("lightHack");
			attacking = false;
		}

		if(attackTick > 0) attackTick -= Time.deltaTime;
		
		if(attacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f && attackTick <= 0) {
			animator.Play(0);
			attackTick = attackDelay;
		}

		if(attacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
			OnAttackFinish();
			attacking = mouseAttack = false;
		}

		for(int i = 0; i < pickedItems.Count; i++)
		if(IsItemGettingPickedUp(i)) {
			var pickedItem = pickedItems[i];
			if(pickedItem == null) continue;
			if(pickedItem.pickedItemCurve < 1) pickedItem.pickedItemCurve += Time.deltaTime;
			else ResetPickup(i);

			if(pickedItem != null) {
				pickedItem.pickedItem.transform.position = Util.Parabola(pickedItem.pickedItem.transform.position, backpack.transform.position + Vector3.up / 2 + pickedItem.targetOffs, 2, pickedItem.pickedItemCurve);
				pickedItem.pickedItem.transform.rotation = Quaternion.Lerp(pickedItem.pickedItem.transform.rotation, pickedItem.targetRot, pickedItem.pickedItemCurve * 0.5f);
				pickedItem.pickedItem.transform.localScale = Vector3.Lerp(pickedItem.pickedItem.transform.localScale, Vector3.one * 0.35f, pickedItem.pickedItemCurve);
			}
		}
	}

	public virtual void TriggerBattle(KnightMovement src) {
		inBattle = true;
		enemy = src;
	}
	public virtual void UntriggerBattle(bool victory = false) {
		enemy = null;
		inBattle = false;
	}

	protected void AttackSwingLeft() {
		swingDirection = 1;
		animator.SetTrigger("leftSwing");
	}
	protected void AttackSwingRight() {
		swingDirection = -1;
		animator.SetTrigger("rightSwing");
	}
	public virtual void AttackKnight(KnightMovement knight) {
		float dmg = knightDMG;
		if(meleeAction == MELEE_ACTION.SWING) dmg *= knightSwingBonusFactor;
		uint dmgi = (uint)dmg;
		float newHealth = (float)(knight.health - dmg);
		if(newHealth < 0) newHealth = 0;
		knight.Hurt((int)dmgi, gameObject);
		if(client != null) client.SendDamageToServer(knight.client.playerIndex, newHealth);
	}

	protected void ResetPickup(int count) {
		pickedItems[count].pickedItem.GetComponent<Ingredient>().owner = this;
		pickedItems[count].pickedItemCurve = 0;
		pickedItems.RemoveAt(count);
	}

	protected virtual void OnAttackFinish() {
		meleeAction = KnightMovement.MELEE_ACTION.NONE;
	}

	public virtual void ResetItemPickup() {}

	public virtual void PickupItem(Ingredient ing, GameObject obj) {
		var rigids = ing.GetComponentsInChildren<Rigidbody>();
		foreach(var rigid in rigids) {
			rigid.isKinematic = true;
			rigid.useGravity = false;
		}
		SoundManager.PLAY_SOUND("takeItem", 0.55f, 1);
		ing.gameObject.transform.position = obj.transform.position;
		pickedItems.Add(new ItemPickup(ing.gameObject, backpack));
		ing.owner = this;
		if(inventory != null) inventory.AddItemToInventory(ing);
	}

	public virtual void NotifyPickup(Ingredient ing, GameObject part) {}
	public virtual void NotifyPlayerLoot(KnightHead knight) {}

	public virtual bool Hurt(int i, GameObject source, bool particles = true) {
		if(hurtTick <= hurtDelay) return false;
		HandleHurt(source, particles);
		health -= i;

		if(health <= 0) {
			Die();
			health = 0;
			return true;
		} else return false;
	}

	protected void HandleHurt(GameObject source, bool particles = true) {
		if(hurtTick <= hurtDelay) return;
		if(particles) SoundManager.PLAY_UNIQUE_SOUND_AT("attackhit", transform.position, 0.55f, 0.25f, 0);
		else SoundManager.PLAY_UNIQUE_SOUND_AT("attackhit", transform.position, 0.55f, 0.1f, 1f);
		lastHurtSource = source;
		hurtBounceAmp = knightBaseScale.y * 1.7f;
		hurtBounceSpeed = 10f;
		hurtTick = 0;
		if(particles) SpawnSeasoningParticles();
	}

	public virtual void SetHealth(float health) {
		if(this.health < health) HandleHurt(null);
		this.health = health;
		if(this.health <= 0) Die();
	}

	public virtual void Heal(int i) { //TODODOODODO: Chompychomp sounds? particles?
		health += i;
		if(health > maxHealth) health = maxHealth;
	}

	protected GameObject SpawnSeasoningParticles() {
		return Instantiate(seasoningParticlesPrefab, transform.position + new Vector3(0, 5, 0), Quaternion.Euler(-90, 0, 0));
	}

	protected void ApplyHurtBounce() {
		animator.transform.localScale = new Vector3(knightBaseScale.x, knightBaseScale.y + Mathf.Sin(hurtTick * hurtBounceSpeed) * hurtBounceAmp, animator.transform.localScale.z);
		hurtBounceAmp = Mathf.Lerp(hurtBounceAmp, 0, Time.deltaTime * 2f);
		hurtBounceSpeed = Mathf.Lerp(hurtBounceSpeed, 0, Time.deltaTime * 2f);
		hurtTick += Time.deltaTime * 5f;
	}

	public virtual void DrainStamina(float i) {
		stamina -= i * staminaDrainFactor;
		if(stamina < 0) stamina = 0;
		if(stamina > maxStamina) stamina = maxStamina;
	}
	public virtual void ChargeStamina(float i) {
		stamina += i;
		if(stamina < 0) stamina = 0;
		if(stamina > maxStamina) stamina = maxStamina;
	}

	public virtual void Die() {
		var headBone = chivalryHeadBone.transform;
		SpawnSeasoningParticles();
		var obj = Instantiate(chivalryHead, headBone.position, Quaternion.Euler(headBone.eulerAngles.x - 90, headBone.eulerAngles.y, headBone.eulerAngles.z));
		var head = obj.GetComponent<KnightHead>();
		head.playerName = gameObject.name;
		head.inventory = inventory;
		Destroy(gameObject);
	}

	public bool IsAnimation(string name) {
		return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == name;
	}
}
