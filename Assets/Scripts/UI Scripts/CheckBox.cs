using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour {
	RectTransform rectTransform;
	public bool isChecked = false;
	private bool canMove = true;

	void Start () {
		rectTransform = GetComponent<RectTransform> ();
		rectTransform.sizeDelta = Vector2.zero;
	}

	public void SwitchCheck(){
		if (!SquareClick.windowOut)
			return;
		if (isChecked)
			StartCoroutine (UnCheck ());
		else
			StartCoroutine (Check ());
	}
	
	IEnumerator Check(){
		if (!canMove) {
			StopCoroutine ("Check");
			yield break;
		}
		canMove = false;
		yield return WorldSelectBanner.AnimateValue (0f, 30f, 0.4f, WorldSelectBanner.OvershootInterpolator, 
			(float value) => {
				rectTransform.sizeDelta = new Vector2(value, value);
			});
		isChecked = true;
		canMove = true;
	}

	IEnumerator UnCheck(){
		if (!canMove) {
			StopCoroutine ("UnCheck");
            yield break;
		}
		canMove = false;
		yield return WorldSelectBanner.AnimateValue (30f, 0f, 0.4f, WorldSelectBanner.AnticipateInterpolator, 
			(float value) => {
				rectTransform.sizeDelta = new Vector2(value, value);
			});
		isChecked = false;
		canMove = true;
	}
}
