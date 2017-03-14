using UnityEngine;

public class P
{
	public static bool isInit = false;
	public static float magicNumber = 0f;

	public static void init(){
		if (isInit)
			return;
		isInit = true;
		GameObject g = new GameObject ();
        Sprite sp = Resources.Load<Sprite>("pixel");
        

		g.AddComponent<SpriteRenderer> ().sprite = sp;
		g.transform.localScale = new Vector3 (Screen.height * Camera.main.aspect, Screen.height, 1f);
		magicNumber = 10f / g.GetComponent<SpriteRenderer> ().bounds.size.y;
		//10 = Camera.main.size*2
		//Debug.Log ("magic " + g.GetComponent<SpriteRenderer> ().bounds.size.y);
		UnityEngine.Object.Destroy(g);
	}

	/**
	 * percent of screen in "Scale" unit (garanteed to work in transform.localScale and Edit mode only)
	 */
	public static float pocSEditor(float percent, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return percent * (Screen.width + Screen.width*0.1f);
		case Side.H:
		case Side.HEIGHT:
			return percent * (Screen.height + Screen.height * 0.1f);
		}
		return -1f;
	}

	/**
	 * percent of screen in "Scale" unit (garanteed to work in transform.localScale and Game mode only)
	 */
	public static float pocSGame(float percent, Side side){
		//new Vector3 (Screen.height*Camera.main.aspect, Screen.height, 1f)
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (percent * (Screen.height*Camera.main.aspect)) * magicNumber;
		case Side.H:
		case Side.HEIGHT:
			return (percent * Screen.height) * magicNumber;
		}
		return -1f;
	}

	/**
	 * percent of screen in "Position" unit (garanteed to work in transform.position only)
	 */
	public static float pocP(float percent, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (percent * (Camera.main.orthographicSize * Camera.main.aspect));
		case Side.H:
		case Side.HEIGHT:
			return (percent * Camera.main.orthographicSize);
		}
		return -1f;
	}

	public static float unPocP(float worldPos, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (worldPos / (Camera.main.orthographicSize * Camera.main.aspect));
		case Side.H:
		case Side.HEIGHT:
			return (worldPos / Camera.main.orthographicSize);
		}
		return -1f;
	}

}

public enum Side
{
	W,
	H, 
	WIDTH,
	HEIGHT
}


