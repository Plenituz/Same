using UnityEngine;
using System.Collections;

public class Explo : MonoBehaviour {
	public float startOffset = 0f;
	public Color color;

	void Start () {
		GetComponent<SpriteRenderer> ().color = color;
		StartCoroutine ("s");
	}

	IEnumerator s(){
		yield return new WaitForSeconds (startOffset);
		StartCoroutine (WorldSelectBanner.AnimateValue (0f, 1f, 0.7f, WorldSelectBanner.AccelerateDeccelerateInterpolator, 
			(float value) => {
				transform.localScale = new Vector3(value*2f, value*2f, 1f);
			}));
		yield return new WaitForSeconds (0.2f);
		StartCoroutine (WorldSelectBanner.AnimateValue (1f, 0f, 0.5f, WorldSelectBanner.AccelerateDeccelerateInterpolator, 
			(float value, object o) => {
				SpriteRenderer sr = (SpriteRenderer) o;
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value);
				if(value == 0f){
					Destroy(gameObject);
				}
			}, GetComponent<SpriteRenderer>()));
	}
}
