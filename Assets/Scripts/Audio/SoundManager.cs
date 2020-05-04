using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SINGLETON approach to the majority of one-shot audio used in the game
public class SoundManager : MonoBehaviour {
	public float AudioLevel = 0.15f, SoundLevel = 1, AmbientLevel = 1;
	private float AudioBase, SoundBase, AmbientBase;

	public Player listener;

	public GameSettings settings;

	protected float crescendo = 0;
	protected float crescendoTarget = 1;
	public static void SetCrescendo(float target) {
		instance.crescendoTarget = target;
	}

	public float GetMasterAudioLevel {
		get {return (settings == null) ? (AudioLevel * crescendo) : (AudioLevel * crescendo) * settings.playerSettings.musicVol;}
	}

	public float GetMasterSoundLevel {
		get {return (settings == null) ? SoundLevel : SoundLevel * settings.playerSettings.soundVol;}
	}

	public float GetMasterAmbientLevel {
		get {return (settings == null) ? AmbientLevel : AmbientLevel * settings.playerSettings.ambientVol;}
	}

	private static SoundManager instance;

	[System.Serializable]
	public class GameOST {
		public string name;
		public AudioClip clip;
		public OSTEvent whenTriggered;	
	}

	[System.Serializable]
	public enum OSTEvent {
		BATTLE, GATHERING, LOADING, CUTSCENE, DEATH
	}

	[Header("Music")]
	public AudioSource OST0;
	public AudioSource OST1;
	public OSTEvent GameOSTState;
	public GameOST[] OST;

	[Header("Sounds")]
	public AudioClip[] sounds;

	private Dictionary<string, AudioClip> soundBank = new Dictionary<string, AudioClip>();
	private Dictionary<OSTEvent, GameOST> OSTBank = new Dictionary<OSTEvent, GameOST>();
	private int OSTFocus = 0;

	[HideInInspector]
	public AmbientNoise[] ambientNoises;
	public AmbientNoise windNoise;

	[Tooltip("Environmental material instances are needed to match the grass/tree swaying frequency to the ambient wind noise frequency")]
	public Material grassMaterial;

	public AnimationCurve windInfluenceCurve;

	void Start() {
		if(instance != null) {
			instance.ambientNoises = ambientNoises;
			instance.windNoise = windNoise;
			instance.soundBank.Clear();
			instance.sounds = sounds;
		    for(int i = 0; i < sounds.Length; i++) instance.soundBank.Add(sounds[i].name.ToLower(), sounds[i]);
			instance.listener = listener;
			instance.UpdateVolumeLevels();
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
		ambientNoises = FindObjectsOfType<AmbientNoise>();
		for(int i = 0; i < sounds.Length; i++) soundBank.Add(sounds[i].name.ToLower(), sounds[i]);
		for(int i = 0; i < OST.Length; i++) OSTBank.Add(OST[i].whenTriggered, OST[i]);

		UpdateVolumeLevels();
		OST0.clip = OSTBank[GameOSTState].clip;
		OST0.volume = AudioLevel;
		OST1.volume = 0;

		OST0.Play();
	}

	private void UpdateVolumeLevels() {
		AudioBase = AudioLevel;
		SoundBase = SoundLevel;
		AmbientBase = AmbientLevel;
		foreach(var ambient in ambientNoises) ambient.masterVolume = GetMasterAmbientLevel; 
	}

	//Smoothly fades in all audio
	void Update() {
		crescendo = Mathf.Lerp(crescendo, crescendoTarget, Time.deltaTime);
		CrossFadeOST();
	}

	//Crossfades the respective themes of mid-battle and 'idle'
	protected void CrossFadeOST() {
		if(!instance.OSTBank.ContainsKey(GameOSTState)) {
			OST0.volume = Mathf.Lerp(OST0.volume, 0, Time.deltaTime);
			OST1.volume = Mathf.Lerp(OST1.volume, 0, Time.deltaTime);
			return;
		}

		if(OSTFocus <= 0) {
			OST0.volume = Mathf.Lerp(OST0.volume, GetMasterAudioLevel, Time.deltaTime);
			OST1.volume = Mathf.Lerp(OST1.volume, 0, Time.deltaTime);
		} else if(OST1.clip != null) {
			OST0.volume = Mathf.Lerp(OST0.volume, 0, Time.deltaTime);
			OST1.volume = Mathf.Lerp(OST1.volume, GetMasterAudioLevel, Time.deltaTime);
		}
	}

	//Sets the OST state depending on game events
	public static void SetOSTState(OSTEvent eve, bool abrupt = false) {
		if(instance == null || instance.GameOSTState == eve) return;
		instance.GameOSTState = eve;

		if(!instance.OSTBank.ContainsKey(eve)) return;

		if(instance.OSTFocus <= 0) {
			instance.OST1.clip = instance.OSTBank[instance.GameOSTState].clip;
			instance.OSTFocus = 1;
			instance.OST1.Play();
			if(abrupt) {
				instance.OST1.volume = instance.GetMasterAudioLevel;
				instance.crescendo = instance.crescendoTarget = 1.25f;
			}
		} else {
			instance.OST0.clip = instance.OSTBank[instance.GameOSTState].clip;
			instance.OSTFocus = 0;
			instance.OST0.Play();
			if(abrupt) {
				instance.OST0.volume = instance.GetMasterAudioLevel;
				instance.crescendo = instance.crescendoTarget = 1.25f;
			}
		}
	}

	void LateUpdate() {
		if(AudioBase != AudioLevel || SoundBase != SoundLevel || AmbientBase != AmbientLevel) UpdateVolumeLevels();
		
		//Updates the grass to match the frequency of the wind noise algorithm
		grassMaterial.SetVector("_WindFrequency", new Vector4(GetWindFrequency() * 0.01f, GetWindFrequency() * -0.05f, 0, 0));
	}

	public static float GetWindFrequency() {
		if(instance == null || instance.windNoise == null) return -5;
		return instance.windInfluenceCurve.Evaluate(instance.windNoise.GetFrequency() / 3000f);
	}

	public static float GetMasterAmbientVolume() {
		return instance.GetMasterAmbientLevel;
	}

	public void PlaySound(string name) {
		if(soundBank.Count <= 0) return;
		if(soundBank.ContainsKey(name.ToLower())) PlayClipAtPoint(soundBank[name.ToLower()], Camera.main.transform.position, 0.5f, 1, 0);
		else Debug.LogError("Could not find '" + name.ToLower() + "' sound file!");
	}

	#region STATIC_ONESHOT_AUDIO
	public static void PLAY_UNIQUE_SOUND(string name, float volume = 1.0f, float range = 0.3f, float basepitch = 0) {
		float pitch = Random.Range(1f - range, 1f + range) + basepitch;
		PLAY_SOUND(name.ToLower(), Camera.main.transform.position - new Vector3(0, 20, 0), volume, pitch);
	}

	public static void PLAY_UNIQUE_SOUND_AT(string name, Vector3 pos, float volume = 1.0f, float range = 0.3f, float basepitch = 0, float spatialBlend = 0.75f) {
		float pitch = Random.Range(1f - range, 1f + range) + basepitch;
		PLAY_SOUND(name.ToLower(), pos, volume, pitch, spatialBlend);
	}

	public static void PLAY_SOUND(string name, float volume = 1.0f, float pitch = 1.0f) {
		PLAY_SOUND(name.ToLower(), Camera.main.transform.position - new Vector3(0, 20, 0), volume, pitch);
	}

	public static void PLAY_SOUND(string name, Vector3 pos, float volume, float pitch, float spatial = 0.75f) {
		if(instance == null) return;
		
		if(instance.soundBank.ContainsKey(name.ToLower())) PlayClipAtPoint(instance.soundBank[name.ToLower()], new Vector3(pos.x, 1, pos.z), volume * instance.GetMasterSoundLevel, pitch, spatial);
		else Debug.LogError("Could not find '" + name.ToLower() + "' sound file!");
	}
	#endregion

	private static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume, float pitch, float spatial) {
		var tempGO = new GameObject("Temp Audio");
		tempGO.transform.position = pos;
		var aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.volume = volume;
		aSource.pitch = pitch;
		aSource.spatialBlend = spatial;
		aSource.dopplerLevel = 0;
		aSource.Play();
		Destroy(tempGO, clip.length);
		return aSource;
	}
}
