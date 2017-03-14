using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SquareClick : MonoBehaviour {
	private BoxCollider2D coll;
	private Reference reference;
	private bool ok = false;
	private GameObject menuCanvas;
	private Text number;
	private Text status;
	public static bool windowOut = false;
	public static bool canMove = true;
	public bool winOutPerso = false;
	public static int selectedLvl = -1;

	public static void Reset(){
		selectedLvl = -1;
		canMove = true;
		windowOut = false;
	}

	void Start () {
		coll = GetComponent<BoxCollider2D> ();
		reference = GetComponent<Reference> ();
		menuCanvas = GameObject.FindGameObjectWithTag ("AlwaysThere");
		number = menuCanvas.transform.FindChild ("Number").GetComponent<Text> ();
		status = menuCanvas.transform.FindChild ("Status").GetComponent<Text> ();
		for (int i = 0; i < menuCanvas.transform.childCount; i++) {
			menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().CrossFadeAlpha (0f, 0f, true);
		}
	}
	
	void Update(){
		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch (0);
			Vector2 worldSpaceTouch = new Vector2 (P.pocP ((t.position.x / Screen.width) * 2f - 1f, Side.W), P.pocP ((t.position.y / Screen.height) * 2f - 1f, Side.H));
			switch (t.phase) {
			case TouchPhase.Began:
				if (coll.bounds.Contains (worldSpaceTouch)) {
					ok = true;
				}
				break;
			case TouchPhase.Ended:
				if (ok && coll.bounds.Contains (worldSpaceTouch)) {
					if (!windowOut && canMove)
						StartCenterSquare ();
				} else {
					if (windowOut && winOutPerso && canMove)
						StartPutBackSquare ();
				}
				ok = false;
				break;
			case TouchPhase.Canceled:
				ok = false;
				break;
			}
		}
		#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0)){
			Vector2 worldSpaceTouch = new Vector2 (P.pocP ((Input.mousePosition.x / Screen.width) * 2f - 1f, Side.W), P.pocP ((Input.mousePosition.y / Screen.height) * 2f - 1f, Side.H));
			if (coll.bounds.Contains (worldSpaceTouch)) {
				if (!windowOut && canMove)
					StartCenterSquare ();
			} else {
				if (windowOut && winOutPerso && canMove)
					StartPutBackSquare ();
			}
		}
		#endif
	}

	void StartCenterSquare(){
		char[] n = gameObject.name.ToCharArray ();
		int x = int.Parse (n [n.Length - 4].ToString ());
		int y = int.Parse (n [n.Length - 2].ToString ());
		StartCoroutine (CentreSquare (x, y));
		//(x + 1 + (Mathf.Abs (y - 4) * 4))
		//TODO reset le rendre order, clickable = true
		//windowOut = false
	}

	void StartPutBackSquare(){
		char[] n = gameObject.name.ToCharArray ();
		int x = int.Parse (n [n.Length - 4].ToString ());
		int y = int.Parse (n [n.Length - 2].ToString ());
		StartCoroutine (PutBackSquare (x, y));
	}

	IEnumerator CentreSquare(int x, int y){
		canMove = false;
		WorldSelectBanner.clickable = false;
		int lvlNb = (x + 1 + (Mathf.Abs (y - 4) * 4));
		Data data = Data.GetInstance ();

		//setup the menu canvas to display the good level number and phrase
		number.text = lvlNb.ToString ();
		switch (data.completedLevels [WorldSelectBanner.mondeActuel - 1, lvlNb - 1]) {
		case Data.LOCKED:
			WorldSelectBanner.clickable = true;
			canMove = true;
			StopCoroutine ("CentreSquare");
			yield break;
		case Data.UNLOCKED:
			status.text = "yet to be completed";
			for (int i = 0; i < menuCanvas.transform.childCount; i++) {
				menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().color = Color.white;
			}
			break;
		case Data.WIN_IN_PUSSY:
			status.text = "completed in easy mode";
			for (int i = 0; i < menuCanvas.transform.childCount; i++) {
				menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().color = Color.black;
			}
			break;
		case Data.WIN_IN_NORMAL:
			status.text = "completed";
			for (int i = 0; i < menuCanvas.transform.childCount; i++) {
				menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().color = Color.black;
			}
			break;
		}
		windowOut = true;
		winOutPerso = true;
		selectedLvl = lvlNb;

		//setting the render order 
		GetComponent<SpriteRenderer> ().sortingOrder = 1;
		reference.GetReferenceByName ("Canvas").GetComponent<Canvas> ().sortingOrder = 2;

		//animating
		float t1 = P.pocSGame(0.15f, Side.W);
		float t2 = P.pocSGame(0.6f, Side.W);
		Vector3 baseScale = new Vector3(t1, t1, 1f);
		Vector3 targetScale = new Vector3(t2, t2, 1f);
		StartCoroutine (WorldSelectBanner.AnimateValue (0f, 1f, 0.4f, WorldSelectBanner.AccelerateDeccelerateInterpolator,
			(float value, object o) => {
				transform.position = Vector3.Lerp((Vector3) ((object[])o)[0], Vector3.zero, value);
				transform.localScale = Vector3.Lerp((Vector3) ((object[])o)[1], (Vector3) ((object[])o)[2], value);
			}, new object[]{transform.position, baseScale, targetScale}));
		yield return new WaitForSeconds (0.2f);
		reference.GetReferenceByName ("Text").GetComponent<Text> ().CrossFadeAlpha (0f, 0.2f, false);
		for (int i = 0; i < menuCanvas.transform.childCount; i++) {
			menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().CrossFadeAlpha (1f, 0.2f, false);
		}
		yield return new WaitForSeconds (0.21f);
		canMove = true;
	}

	IEnumerator PutBackSquare(int x, int y){
		canMove = false;
		//animating
		float t1 = P.pocSGame(0.6f, Side.W);
		float t2 = P.pocSGame(0.15f, Side.W);
		Vector3 baseScale = new Vector3(t1, t1, 1f);
		Vector3 targetScale = new Vector3(t2, t2, 1f);
		StartCoroutine (WorldSelectBanner.AnimateValue (0f, 1f, 0.4f, WorldSelectBanner.AccelerateDeccelerateInterpolator,
			(float value, object o) => {
				transform.position = Vector3.Lerp(Vector3.zero, (Vector2) ((object[])o)[0], value);
				transform.localScale = Vector3.Lerp((Vector3) ((object[])o)[1], (Vector3) ((object[])o)[2], value);
			}, new object[]{WorldSelectBanner.defaultPositions[x, y], baseScale, targetScale}));
		
		for (int i = 0; i < menuCanvas.transform.childCount; i++) {
			menuCanvas.transform.GetChild (i).GetComponent<Graphic> ().CrossFadeAlpha (0f, 0.2f, false);
		}
		yield return new WaitForSeconds (0.2f);
		reference.GetReferenceByName ("Text").GetComponent<Text> ().CrossFadeAlpha (1f, 0.2f, false);
		yield return new WaitForSeconds (0.21f);

		//setting the render order 
		GetComponent<SpriteRenderer> ().sortingOrder = 0;
		reference.GetReferenceByName ("Canvas").GetComponent<Canvas> ().sortingOrder = 0;
		//
		windowOut = false;
		winOutPerso = false;
		WorldSelectBanner.clickable = true;
		canMove = true;
	}
}
