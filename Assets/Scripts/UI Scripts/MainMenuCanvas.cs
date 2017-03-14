using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
	public Transform parent;
	private RectTransform r;
	private Vector2 basePos;

	void Start () {
		r = GetComponent<RectTransform> ();
		basePos = r.anchoredPosition;
		r.anchoredPosition = (Vector2) parent.position + basePos;
	}

	void Update(){
		r.anchoredPosition = (Vector2) parent.position + basePos;
	}
}
