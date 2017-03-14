using UnityEngine;
using System.Collections;

public class PowerScript : MonoBehaviour {
	public Power power;

	public void OnClick(){
		power.Trigger ();
		if (PlayerScript.onColorSwitchSignal != null)
			PlayerScript.onColorSwitchSignal ();
	}

	public void ScaleOut(){
		StartCoroutine (WorldSelectBanner.AnimateValue (transform.localScale.x, 0f, 0.6f, WorldSelectBanner.AccelerateDeccelerateInterpolator, 
			(float value) => {
				transform.localScale = new Vector3(value, value, 1f);
			}));
	}

	#if UNITY_EDITOR
	void Update(){
		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Alpha3) || Input.GetKeyDown (KeyCode.Alpha4)) {
			switch (name.ToCharArray () [5]) {
			case '1':
				if (Input.GetKeyDown (KeyCode.Alpha1)) {
					power.Trigger ();
				}
				break;
			case '2':
				if (Input.GetKeyDown (KeyCode.Alpha2)) {
					power.Trigger ();
				}
				break;
			case '3':
				if (Input.GetKeyDown (KeyCode.Alpha3)) {
					power.Trigger ();
				}
				break;
			case '4':
				if (Input.GetKeyDown (KeyCode.Alpha4)) {
					power.Trigger ();
				}
				break;
			}
		}
	}
	#endif
}
