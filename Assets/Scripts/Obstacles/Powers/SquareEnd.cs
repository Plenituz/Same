using UnityEngine;
using System.Collections;

public class SquareEnd : MonoBehaviour {
	public Color color;

	void Start () {
		GetComponent<SpriteRenderer> ().color = color;
		float endX = P.pocSGame (1f, Side.W);
		float endY = P.pocSGame (1f, Side.H);
		StartCoroutine (WorldSelectBanner.AnimateValue (0f, 1f, 0.7f, WorldSelectBanner.AccelerateDeccelerateInterpolator, 
			(float value) => {
				transform.localScale = new Vector3(value*endX, value*endY, 1f);
			}));
	}
}
