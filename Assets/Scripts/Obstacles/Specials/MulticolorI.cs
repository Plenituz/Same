using UnityEngine;
using System.Collections;

public class MulticolorI : MonoBehaviour {
	private Multicolorable go;
	private Color nextColor;
	private Color color;

	void Start(){
		nextColor = new Color (Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), 1f);
		color = nextColor;
	}

	void Update(){
		if (go == null) {
			go = GetComponent<Multicolorable> ();
		}
		if (Mathf.Floor(color.r*100) == Mathf.Floor(nextColor.r*100)) {
			nextColor = new Color (Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), 1f);
		}
		color = Color.Lerp (color, nextColor, 0.1f);
		go.SetColor (color);
	}
}
