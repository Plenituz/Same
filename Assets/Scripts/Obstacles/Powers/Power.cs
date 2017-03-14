using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public enum Powers{
	Null,
	Boom,
	SlowTime,
	United,
	Laser,
	Invincible,
	Ultimate,
	End
}

[System.Serializable]
public class PowerAttribut{

	public PowerAttribut(Powers t, float cool, float effect, int use){
		type = t;
		unlocked = false;
		level = 1;
		cooldownDuration = cool;
		effectDuration = effect;
		useBeforeLevelUp = use;
	}

	public Powers type;
	public bool unlocked = false;
	public int level = 0;
	public float cooldownDuration;
	public float effectDuration;
	public int useBeforeLevelUp;
}

public abstract class Power {
	public float startTime;
	public Sprite icon;
	public float cooldownDuration;
	public float effectDuration;
	public bool enabled = true;
	public Runnable onEnable;

	protected Slider slider;
	private bool hasEnded = false;
	protected PowerAttribut attrib;

	public Power(Slider slider){
		this.slider = slider;
		icon = GetIcon ();
	}

	public void SetAttribut(PowerAttribut attrib){
		cooldownDuration = attrib.cooldownDuration;
		effectDuration = attrib.effectDuration;
		this.attrib = attrib;
	}

	public void UpdateCooldownGraphics (){
		//update graphics
		slider.value = cooldownDuration - (Time.time - startTime);

		if (Time.time - startTime > cooldownDuration*Time.timeScale) {
			GameStartup.onUpdate -= UpdateCooldownGraphics;
			slider.value = slider.maxValue;
			Enable ();
		}

		if (!hasEnded) {
			EffectUpdate ();
			if (Time.time - startTime > effectDuration * Time.timeScale) {
				hasEnded = true;
				EffectEnd ();
			}
		}
	}

	public void Trigger(){
		if (!enabled)
			return;
		startTime = Time.time;
		Disable ();
		GameStartup.onUpdate += UpdateCooldownGraphics;
		EffectStart ();
		slider.maxValue = cooldownDuration;
		slider.value = cooldownDuration;
		attrib.useBeforeLevelUp--;
		Data.SaveData ();
	}

	public void Enable(){
		enabled = true;
		if (onEnable != null)
			onEnable ();
	}

	public void Disable(){
		enabled = false;
	}
		
	public abstract Sprite GetIcon();
	public abstract void EffectStart();
	public abstract void EffectUpdate();
	public abstract void EffectEnd();

	public static Power GetInstance(Powers pow, Slider slider){
		switch (pow) {
		case Powers.Boom:
			return new BoomPower (slider);
		case Powers.SlowTime:
			return new SlowTime (slider);
		case Powers.United:
			return new UnitedPower (slider);
		case Powers.Laser:
			return new LaserPower (slider, 5f);
		case Powers.Invincible:
			return new InvinciblePower (slider); 
		case Powers.Ultimate:
			return new UltimatePower (slider);
		default:
			return null;
		}
	}
}

public class BoomPower : Power {

	public BoomPower(Slider slider) : base (slider) {}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/Boom");
	}

	public override void EffectStart(){
		GameObject explPrefab = Resources.Load<GameObject> ("PowerIcons/ExploGroup");
		GameObject explo = GameObject.Instantiate (explPrefab) as GameObject;
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		explo.transform.position = player.transform.position;

		GameObject[] allObjs = GameObject.FindGameObjectsWithTag ("Obstacle");
		for (int i = 0; i < allObjs.Length; i++) {
			GameObject.Destroy (allObjs [i]);
		}
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){}
}

public class SlowTime : Power {

	public SlowTime(Slider slider) : base (slider) {}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/Slow");
	}

	public override void EffectStart(){
		Time.timeScale = 0.5f;
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){
		Time.timeScale = 1f;
	}
}

public class UnitedPower : Power {

	public UnitedPower(Slider slider) : base (slider) {}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/United");
	}

	public override void EffectStart(){
		GameStartup startup = Camera.main.gameObject.GetComponent<GameStartup> ();
		Color c = GameObject.FindGameObjectWithTag ("Player").GetComponent<MeshRenderer> ().material.color;
		for (int i = 0; i < startup.obstacles.Count; i++) {
			GameObject o = startup.obstacles [i] as GameObject;
			float obsStartAt = o.GetComponent<GameObstacle> ().startAtTime;
			if ((startup.startTime + obsStartAt) > Time.time && (startup.startTime + obsStartAt) < Time.time + effectDuration) {
				o.GetComponent<GameObstacle> ().SetColor (c);
			}
		}
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){}
}

public class LaserPower : Power {
	private float speed;

	public LaserPower(Slider slider, float speed) : base(slider) {
		this.speed = speed;
	}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/Laser");
	}

	public override void EffectStart(){
		GameObject laser = GameObject.Instantiate (Resources.Load<GameObject> ("PowerIcons/LaserObj")) as GameObject;
		Laser laserScript = laser.GetComponent<Laser> ();
		float angle = Random.Range (0f, Mathf.PI * 2);
		laser.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Rad2Deg*angle));
		laser.transform.position = GameObject.FindGameObjectWithTag ("Player").transform.position;
		laserScript.velocity = new ComplexNumber (speed, angle, ComplexNumber.GEOMETRICAL).toVector ();
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){}
}

public class InvinciblePower : Power {
	private Runnable saved;
	private MulticolorI mn;
	private PlayerScript player;
	private Color savedColor;
	
	public InvinciblePower(Slider slider) : base(slider) {}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/Invincible");
	}

	public override void EffectStart(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerScript> ();
		mn = player.gameObject.AddComponent<MulticolorI> ();
		savedColor = player.GetColor ();
		saved = PlayerScript.onDeath;
		PlayerScript.onDeath = null;
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){
		player.SetColor (savedColor);
		GameObject.Destroy (mn);
		PlayerScript.onDeath += saved;
	}
}

public class UltimatePower : Power {
	
	public UltimatePower(Slider slider) : base(slider) {
	}

	public override Sprite GetIcon(){
		return Resources.Load<Sprite> ("PowerIcons/Ultimate");
	}

	public override void EffectStart(){
		Power p = Power.GetInstance ((Powers) Random.Range ((int)Powers.Boom, (int)Powers.Ultimate), slider);
		Debug.Log (p.GetType ());
		p.SetAttribut (attrib);
		p.Trigger ();
	}

	public override void EffectUpdate(){}

	public override void EffectEnd(){}
}
