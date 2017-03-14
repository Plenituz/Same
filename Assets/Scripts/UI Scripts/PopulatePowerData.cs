using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopulatePowerData : MonoBehaviour {

	void Start () {
		Data data = Data.GetInstance ();
		Text nextLvl = transform.FindChild ("Next level in...").GetComponent<Text> ();
		Text desc = transform.FindChild ("Desc").GetComponent<Text> ();
		Text cooldown = transform.FindChild ("Cooldown").GetComponent<Text> ();
		Text level = transform.FindChild ("Level").GetComponent<Text> ();

		PowerAttribut pow = null;
		for (int i = 0; i < data.powerAttributs.Length; i++) {
			if (data.powerAttributs [i].type.ToString ().Equals (name)) {
				pow = data.powerAttributs [i];
				break;
			}
		}
		level.text = pow.level.ToString ();
		cooldown.text = "Recharge\n" + pow.cooldownDuration + " sec";
		nextLvl.text = pow.unlocked ? pow.level == 3 ? ("No more\nupgrades") : ("Next level\nin " + pow.useBeforeLevelUp + " uses") : ("Play more to\nunlock");
		desc.text = string.Format (desc.text, pow.effectDuration);
	}

	public void GoToEquip(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("EquipPower");
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainMenu");
		}
	}
}
