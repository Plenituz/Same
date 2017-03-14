using UnityEngine;
using System.Collections;

public class RandomSwitch : MonoBehaviour {
	public float minTimeBeforeSwitch = 1f;
	public float maxTimeBeforeSwitch = 5f;
	public Color startColor = Color.black;

	private float startTime;
	private float waitTime;
	private GameObstacle go;
	private Color color;

	void Start () {
		go = GetComponent<GameObstacle> ();
		startTime = Time.time;
		waitTime = Random.Range (minTimeBeforeSwitch, maxTimeBeforeSwitch);
		color = startColor;
		go.SetColor (color);
	}
	
	void Update () {
		if (go == null) {
			go = GetComponent<GameObstacle> ();
		}
		if(go.GetColor() != color)
			go.SetColor (color);
		if (Time.time - startTime >= waitTime) {
			startTime = Time.time;
			waitTime = Random.Range (minTimeBeforeSwitch, maxTimeBeforeSwitch);
			color = color.r == 0f ? Color.white : Color.black;
			go.SetColor (color);
		}
	}
}