using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class WorldSelectBanner : MonoBehaviour {
	public bool down = false;
	public Sprite pixel;
	public static int mondeActuel = 1;
	public static int maxWorld = 2;
	private Color[] worldBGColors = new Color[]{new Color(0.22352941176470588235294117647059f, 0.32549019607843137254901960784314f, 0.64313725490196078431372549019608f, 1f), 
		new Color(0.64705882352941176470588235294118f, 0.21568627450980392156862745098039f, 0.21568627450980392156862745098039f, 1f), 
		new Color(0.23529411764705882352941176470588f, 0.18039215686274509803921568627451f, 0.11372549019607843137254901960784f, 1f) };
	private static Reference bannerUp;
	private static Reference bannerDown;
	private static SpriteRenderer srBG;
	private BoxCollider2D coll;
	private bool ok = false;
	public static bool clickable = true;
	public static GameObject[,] squares = new GameObject[4, 5];
	public static Vector2[,] defaultPositions = new Vector2[4, 5];

	private const float BANNER_UP_Y = 0.85f;
	private const float BANNER_DOWN_Y = -0.94f;
	private const float BANNER_UP_SCALE = 0.08f;
	private const float BANNER_DOWN_SCALE = 0.06f;

	public static void Reset(){
		clickable = true;
		squares = new GameObject[4, 5];
		defaultPositions = new Vector2[4, 5];
		bannerUp = null;
		bannerDown = null;
		srBG = null;
		mondeActuel = 1;
	}

	void Start () {
		PlayerScript.Reset ();
		if (!P.isInit)
			P.init ();
		coll = GetComponent<BoxCollider2D> ();
		if (down) {
			transform.position = new Vector2 (0f, P.pocP (-0.94f, Side.H));
			transform.localScale = new Vector3 (P.pocSGame (1f, Side.W), P.pocSGame (0.06f, Side.H), 1f);
		} else {
			transform.position = new Vector2 (0f, P.pocP (0.85f, Side.H));
			transform.localScale = new Vector3 (P.pocSGame (1f, Side.W), P.pocSGame (0.08f, Side.H), 1f);
		}
		if (bannerUp == null) {
			CreateSquares ();
			srBG = GameObject.FindGameObjectWithTag ("BG").GetComponent<SpriteRenderer> ();
			GameObject[] gs = GameObject.FindGameObjectsWithTag ("Banner");
			foreach (GameObject g in gs) {
				Reference r = g.GetComponent<Reference> ();
				if (r.gameObject.name.Equals ("BannerUp")) {
					bannerUp = r;
				} else {
					bannerDown = r;
				}
			}
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("MainMenu");
		}
		if (squares [0, 0].GetComponent<Reference> ().GetReferenceByName ("Canvas").GetComponent<ParentCanvasPosition> ().parent == null) {
			for (int y = 0; y < 5; y++) {
				for (int x = 0; x < 4; x++) {
					squares [x, y].GetComponent<Reference> ().GetReferenceByName ("Canvas").GetComponent<ParentCanvasPosition> ().parent = squares [x, y].transform;
				}
			}
		}
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
				if (ok && coll.bounds.Contains (worldSpaceTouch))
					OnClick ();
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
			if(coll.bounds.Contains(worldSpaceTouch))
				OnClick();
		}
		#endif
	}

	void OnClick(){
		if (!clickable)
			return;
		
		switch (gameObject.name) {
		case "BannerUp":
			if (mondeActuel != 1)
				StartCoroutine (AnimateUpToDown ());
			break;
		case "BannerDown":
			if (mondeActuel != maxWorld)
				StartCoroutine (AnimateDownToUp ());
			break;
		}
	}

	//
	//TODO check si c'est le premier ou dernier monde et ne pas afficher les fleches en haut ou le bouton en bas
	// faire ca dans le Main "Animatetruc to truc"
	//

	void CreateSquares(){
		int c = 1;
		for (int y = 0; y < 5; y++) {
			for (int x = 0; x < 4; x++) {
				GameObject square = new GameObject ("s[" + c + "," + x + "," + y + "]");
				square.AddComponent<SpriteRenderer> ().sprite = pixel;
				square.AddComponent<SetColorSRGameObstacle> ();
				square.AddComponent<BoxCollider2D> ().isTrigger = true;
				square.AddComponent<SquareClick> ();

				Reference r = square.AddComponent<Reference> ();
				GameObject canvas = Instantiate (Resources.Load ("CanvasModel")) as GameObject;
				r.names = new string[] { "Canvas", "Text" };
				r.objects = new GameObject[]{ canvas, canvas.transform.GetChild (0).gameObject };

				canvas.transform.GetChild (0).gameObject.GetComponent<Text> ().text = (x + 1 + (Mathf.Abs (y - 4) * 4)).ToString ();
				canvas.transform.GetChild (0).gameObject.GetComponent<RectTransform> ().position += new Vector3 (0f, P.pocP (0.01f, Side.H), 0f);

				square.transform.localScale = new Vector3 (P.pocSGame (0.15f, Side.W), P.pocSGame (0.15f, Side.W), 1f);
				square.transform.position = new Vector2 (P.pocP ((0.125f + (x * 0.25f))*2-1f, Side.W), P.pocP (  0.28f + (y * 0.165f)*2-1f, Side.H));
				canvas.GetComponent<RectTransform> ().position = square.transform.position;
				squares [x, y] = square;
				defaultPositions [x, y] = square.transform.position;
				c++;
			}
		}
		UpdateSquareColorForWorld (mondeActuel);
	}

	IEnumerator AnimateDownToUp(){
		//bouger les carré/lvl ici
		//et le bg
		clickable = false;
		StartCoroutine (MoveSquaresAndBGUp ());
		yield return new WaitForSeconds (0.6f);
		StartCoroutine( MoveDownBannerUp ());//pousse la baniere du bas a la place de celle du haut
		yield return MoveUpBannerUp ();//pousse la baniere du haut hors screen
		if (mondeActuel + 1 == maxWorld) {
			bannerUp.GetReferenceByName ("LinkedText").GetComponent<Text> ().text = "Coming soon";
		}
		yield return new WaitForSeconds (0.5f);
		UpdateSquareColorForWorld (mondeActuel + 1);
		StartCoroutine (MoveSquaresInPlaceFromDown ());
		yield return MoveUpBannerDownFromDown ();//teleporte la banniere du haut (hors screen) en bas et la fait monter a la place de la baniere du bas
		//a ce point les deux banieres sont en place mais inversées
		SwitchBanners ();//echange les propriétés des deux banieres pour que 
		//update monde actuel etc
		mondeActuel++;
		clickable = true;
	}

	IEnumerator MoveSquaresAndBGUp(){
		StartCoroutine (MoveSquaresUp ());
		yield return LerpBGColor(worldBGColors[mondeActuel-1], worldBGColors[mondeActuel]);
	}

	IEnumerator MoveSquaresUp(){
		for (int y = 4; y >= 0; y--) {
			for (int x = 0; x < 4; x++) {
				StartCoroutine (AnimateValue (defaultPositions [x, y].y, defaultPositions [x, y].y + P.pocP (2.0f, Side.H), 0.8f, AnticipateInterpolator,
					(float value, object o) => {
						GameObject square = (GameObject) o;
						square.transform.position = new Vector2(square.transform.position.x, value);
					}, squares[x, y]));
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	IEnumerator MoveSquaresInPlaceFromDown(){
		for (int y = 4; y >= 0; y--) {
			for (int x = 0; x < 4; x++) {
				StartCoroutine (AnimateValue (defaultPositions [x, y].y - P.pocP (2.0f, Side.H), defaultPositions [x, y].y, 0.8f, OvershootInterpolator,
					(float value, object o) => {
						GameObject square = (GameObject) o;
						square.transform.position = new Vector2(square.transform.position.x, value);
					}, squares [x, y]));
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	IEnumerator LerpBGColor(Color from, Color to){
		yield return AnimateValue (0f, 1f, 0.8f, null, 
			(float value) => {
				srBG.color = Color.Lerp(from, to, value);
			});
	}

	IEnumerator MoveUpBannerUp(){
		//pousse la baniere du haut hors screen
		//et change le text a la fin de l'animation

		//position initial : la baniere du haut est en haut
		yield return AnimateValue (BANNER_UP_Y, 1.2f, 0.4f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				bannerUp.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			});
		bannerUp.GetReferenceByName ("LinkedText").GetComponent<Text> ().text = "Monde " + (mondeActuel + 2);
	}

	IEnumerator MoveUpBannerDownFromDown(){
		//teleporte la banniere du haut (hors screen) en bas et la fait monter a la place de la baniere du bas
		//et fait tourner les fleches smoothement

		//position initiale : la baniere du haut est hors screen
		Text bannerUpText = bannerUp.GetReferenceByName("LinkedText").GetComponent<Text>();
		RectTransform leftArrow = bannerUp.GetReferenceByName ("ArrowLeft").GetComponent<RectTransform> ();
		RectTransform rightArrow = bannerUp.GetReferenceByName ("ArrowRight").GetComponent<RectTransform> ();

		bannerUp.transform.localScale = new Vector3 (P.pocSGame (1f, Side.W), P.pocSGame (BANNER_DOWN_SCALE, Side.H), 1f);
		StartCoroutine (AnimateValue (-1.2f, BANNER_DOWN_Y, 0.4f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerUp.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			}));
		//la police est grosse et on la fait rapetisser
		StartCoroutine (AnimateValue (60f, 50f, 0.4f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerUpText.fontSize = Mathf.RoundToInt(value);
			}));
		//on fait tourner les fleches
		leftArrow.gameObject.SetActive(true);
		rightArrow.gameObject.SetActive (true);
		yield return AnimateValue (180f, 0f, 0.4f, AccelerateDeccelerateInterpolator,
			(float value) => {
				leftArrow.rotation = Quaternion.Euler (new Vector3 (0f, 0f, value));
				rightArrow.rotation = leftArrow.rotation;
			});

	}

	IEnumerator MoveDownBannerUp(){
		//pousse la baniere du bas a la place de celle du haut
		// et agrandis la taille de la police et fait tourner les fleches
		Text bannerDownText = bannerDown.GetReferenceByName("LinkedText").GetComponent<Text>();
		RectTransform leftArrow = bannerDown.GetReferenceByName ("ArrowLeft").GetComponent<RectTransform> ();
		RectTransform rightArrow = bannerDown.GetReferenceByName ("ArrowRight").GetComponent<RectTransform> ();

		StartCoroutine (AnimateValue (BANNER_DOWN_Y, BANNER_UP_Y, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerDown.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			}));
		StartCoroutine (AnimateValue (50f, 60f, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerDownText.fontSize = Mathf.RoundToInt(value);
			}));
		StartCoroutine (AnimateValue (BANNER_DOWN_SCALE, BANNER_UP_SCALE, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerDown.transform.localScale = new Vector3(P.pocSGame(1f, Side.W), P.pocSGame(value, Side.H), 1f);
			}));
		yield return AnimateValue (0f, 180f, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				leftArrow.rotation = Quaternion.Euler (new Vector3 (0f, 0f, value));
				rightArrow.rotation = Quaternion.Euler (new Vector3 (0f, 0f, value));
			});
	}

	void SwitchBanners(){
		//echange de place les banieres mais en gardant l'affichage identique
		Text upText = bannerUp.GetReferenceByName("LinkedText").GetComponent<Text>();
		Text downText = bannerDown.GetReferenceByName ("LinkedText").GetComponent<Text> ();
		SpriteRenderer srUp = bannerUp.GetComponent<SpriteRenderer> ();
		SpriteRenderer srDown = bannerDown.GetComponent<SpriteRenderer> ();
		GameObject leftArrowDown = bannerDown.GetReferenceByName ("ArrowLeft");
		GameObject rightArrowDown = bannerDown.GetReferenceByName ("ArrowRight");
		GameObject leftArrowUp = bannerUp.GetReferenceByName ("ArrowLeft");
		GameObject rightArrowUp = bannerUp.GetReferenceByName ("ArrowRight");
		Image leftArrowDownImg = leftArrowDown.GetComponent<Image> ();
		Image rightArrowDownImg = rightArrowDown.GetComponent<Image> ();
		Image leftArrowUpImg = leftArrowUp.GetComponent<Image> ();
		Image rightArrowUpImg = rightArrowUp.GetComponent<Image> ();

		//positions
		Vector3 tmpPos = bannerUp.transform.position;
		Vector3 tmpSc = bannerUp.transform.localScale;
		bannerUp.transform.position = bannerDown.transform.position;
		bannerDown.transform.position = tmpPos;
		bannerUp.transform.localScale = bannerDown.transform.localScale;
		bannerDown.transform.localScale = tmpSc;
		//colors
		Color tmpCol = srUp.color;
		srUp.color = srDown.color;
		srDown.color = tmpCol;
		tmpCol = upText.color;
		upText.color = downText.color;
		downText.color = tmpCol;
		tmpCol = leftArrowDownImg.color;
		leftArrowDownImg.color = leftArrowUpImg.color;
		leftArrowUpImg.color = tmpCol;
		rightArrowDownImg.color = leftArrowDownImg.color;
		rightArrowUpImg.color = leftArrowUpImg.color;
		//rotations, font size
		int tmpVal = downText.fontSize;
		downText.fontSize = upText.fontSize;
		upText.fontSize = tmpVal;
		Quaternion tmpQ = leftArrowDownImg.rectTransform.rotation;
		leftArrowDownImg.rectTransform.rotation = leftArrowUpImg.rectTransform.rotation;
		leftArrowUpImg.rectTransform.rotation = tmpQ;
		rightArrowUpImg.rectTransform.rotation = leftArrowUpImg.rectTransform.rotation;
		rightArrowDownImg.rectTransform.rotation = leftArrowDownImg.rectTransform.rotation;
		//text
		string tmpStr = downText.text;
		downText.text = upText.text;
		upText.text = tmpStr;
		//arrow visibilité
		bool tmpBo = leftArrowDown.activeSelf;
		leftArrowDown.SetActive (leftArrowUp.activeSelf);
		leftArrowUp.SetActive (tmpBo);
		rightArrowUp.SetActive (leftArrowUp.activeSelf);
		rightArrowDown.SetActive (leftArrowDown.activeSelf);

	}

	IEnumerator AnimateUpToDown(){
		clickable = false;
		StartCoroutine (MoveSquaresDownAndBGDown ());
		yield return new WaitForSeconds (0.6f);
		StartCoroutine(MoveDownBannerDown ());
		StartCoroutine(MoveDownBannerUpFromUp ());
		StartCoroutine( MoveUpBannerDown ());
		yield return new WaitForSeconds (0.5f);
		UpdateSquareColorForWorld (mondeActuel - 1);
		yield return MoveSquaresInPlaceFromUp ();
		yield return new WaitForSeconds (0.1f);
		SwitchBanners ();
		mondeActuel--;
		clickable = true;
	}

	IEnumerator MoveSquaresDownAndBGDown(){
		StartCoroutine (MoveSquaresDown ());
		yield return LerpBGColor (worldBGColors [mondeActuel - 1], worldBGColors [mondeActuel - 2]); 
	}

	IEnumerator MoveSquaresDown(){
		for (int y = 0; y < 5; y++) {
			for (int x = 0; x < 4; x++) {
				StartCoroutine (AnimateValue (defaultPositions [x, y].y, defaultPositions [x, y].y - P.pocP (2.0f, Side.H), 0.8f, AnticipateInterpolator,
					(float value, object o) => {
						GameObject square = (GameObject) o;
						square.transform.position = new Vector2(square.transform.position.x, value);
					}, squares[x, y]));
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	IEnumerator MoveSquaresInPlaceFromUp(){
		for (int y = 0; y < 5; y++) {
			for (int x = 0; x < 4; x++) {
				StartCoroutine (AnimateValue (defaultPositions [x, y].y + P.pocP (2.0f, Side.H), defaultPositions [x, y].y, 0.8f, OvershootInterpolator,
					(float value, object o) => {
						GameObject square = (GameObject) o;
						square.transform.position = new Vector2(square.transform.position.x, value);
					}, squares[x, y]));
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	IEnumerator MoveDownBannerDown(){
		//pousse la baniere du bas en dehors de l'ecran
		//et change le text a la fin de l'anim
		yield return AnimateValue (BANNER_DOWN_Y, -1.2f, 0.4f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				bannerDown.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			});
		bannerDown.GetReferenceByName ("LinkedText").GetComponent<Text> ().text = "Monde " + (mondeActuel - 1);
	}

	IEnumerator MoveUpBannerDown(){
		//pousse la baniere du haut en bas et tourne les fleches
		//et diminue la police
		//et fait tourner/apparaitre/disparaitre les fleches smoothement
		Text upText = bannerUp.GetReferenceByName("LinkedText").GetComponent<Text>();
		RectTransform arrowLeft = bannerUp.GetReferenceByName ("ArrowLeft").GetComponent<RectTransform> ();
		RectTransform arrowRight = bannerUp.GetReferenceByName ("ArrowRight").GetComponent<RectTransform> ();

		StartCoroutine (AnimateValue (BANNER_UP_Y, BANNER_DOWN_Y, 0.8f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				bannerUp.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			}));
		StartCoroutine (AnimateValue (BANNER_UP_SCALE, BANNER_DOWN_SCALE, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				bannerUp.transform.localScale = new Vector3(P.pocSGame(1f, Side.W), P.pocSGame(value, Side.H), 1f);
			}));
		StartCoroutine (AnimateValue (60f, 50f, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				upText.fontSize = Mathf.RoundToInt(value);
			}));

		yield return AnimateValue(180f, 0f, 0.8f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				arrowLeft.rotation = Quaternion.Euler(new Vector3(0f, 0f, value));
				arrowRight.rotation = arrowLeft.rotation;
			});
	}

	IEnumerator MoveDownBannerUpFromUp(){
		//teleporte la baniere du bas en haut et la fait descendre a la place de la baniere du haut
		//et fait tourner les fleches smoothement
		//et fait grandir la police
		Text downText = bannerDown.GetReferenceByName("LinkedText").GetComponent<Text>();
		RectTransform leftArrow = bannerDown.GetReferenceByName ("ArrowLeft").GetComponent<RectTransform> ();
		RectTransform rightArrow = bannerDown.GetReferenceByName ("ArrowRight").GetComponent<RectTransform> ();

		StartCoroutine (AnimateValue (1.2f, BANNER_UP_Y, 0.8f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				bannerDown.transform.position = new Vector2(0f, P.pocP(value, Side.H));
			}));
		StartCoroutine (AnimateValue (50f, 60f, 0.8f, AccelerateDeccelerateInterpolator,
			(float value) => {
				downText.fontSize = Mathf.RoundToInt(value);
			}));
		if (mondeActuel - 1 != 1) {
			StartCoroutine (AnimateValue (0f, 180f, 0.8f, AccelerateDeccelerateInterpolator, 
				(float value) => {
					rightArrow.rotation = Quaternion.Euler (new Vector3 (0f, 0f, value));
					leftArrow.rotation = rightArrow.rotation;
				}));
		} else {
			rightArrow.gameObject.SetActive (false);
			leftArrow.gameObject.SetActive (false);
		}

		yield return AnimateValue (BANNER_DOWN_SCALE, BANNER_UP_SCALE, 0.4f, AccelerateDeccelerateInterpolator, 
			(float value) => {
				bannerDown.transform.localScale = new Vector3(P.pocSGame(1f, Side.W), P.pocSGame(value, Side.H), 1f);
			});
	}

	void UpdateSquareColorForWorld(int w){
		Data data = Data.GetInstance ();
		for (int y = 0; y < 5; y++) {
			for (int x = 0; x < 4; x++) {
				int completion = data.completedLevels [w - 1, (x + (Mathf.Abs (y - 4) * 4))];
				switch (completion) {
				case Data.LOCKED:
					Destroy (squares [x, y].GetComponent<Multicolor> ());
					squares [x, y].GetComponent<SpriteRenderer> ().color = new Color(0.756f, 0.01f, 0.01f, 1f);
					squares [x, y].GetComponent<Reference> ().GetReferenceByName ("Text").GetComponent<Text> ().color = Color.black;
					break;
				case Data.UNLOCKED:
					Destroy (squares [x, y].GetComponent<Multicolor> ());
					squares [x, y].GetComponent<SpriteRenderer> ().color = Color.black;
					squares [x, y].GetComponent<Reference> ().GetReferenceByName ("Text").GetComponent<Text> ().color = Color.white;
					break;
				case Data.WIN_IN_PUSSY:
					Destroy (squares [x, y].GetComponent<Multicolor> ());
					squares [x, y].GetComponent<SpriteRenderer> ().color = Color.white;
					squares [x, y].GetComponent<Reference> ().GetReferenceByName ("Text").GetComponent<Text> ().color = Color.black;
					break;
				case Data.WIN_IN_NORMAL:
					if (squares [x, y].GetComponent<Multicolor> () == null)
						squares [x, y].AddComponent<Multicolor> ();
					squares [x, y].GetComponent<Reference> ().GetReferenceByName ("Text").GetComponent<Text> ().color = Color.black;
					break;
				}
			}
		}
	}

	public static IEnumerator AnimateValue(float from, float to, float duration, Interpolator interpolator, OnAnimationUpdate oau){
		float startTime = Time.time;
		while (Time.time - startTime <= duration) {
			float value;
			if(interpolator != null)
				value = (from + (to - from)*interpolator((Time.time - startTime)/duration));
			else
				value = (from + (to - from)*((Time.time - startTime)/duration));
			if (oau != null)
				oau (value);
			yield return new WaitForEndOfFrame ();
		}
		if (oau != null)
			oau (to);
	}

	public static IEnumerator AnimateValue(float from, float to, float duration, Interpolator interpolator, OnAnimationUpdateObj oau, object o){
		float startTime = Time.time;
		while (Time.time - startTime <= duration) {
			float value;
			if(interpolator != null)
				value = (from + (to - from)*interpolator((Time.time - startTime)/duration));
			else
				value = (from + (to - from)*((Time.time - startTime)/duration));
			if (oau != null)
				oau (value, o);
			yield return new WaitForEndOfFrame ();
		}
		if (oau != null)
			oau (to, o);
	}

	public static float AccelerateDeccelerateInterpolator(float input){
		return (Mathf.Cos ((input + 1) * Mathf.PI) / 2.0f) + 0.5f;
	}

	public static float OvershootInterpolator(float t){
		t -= 1.0f;
		//2.0f can be replaced by tension
		return t * t * ((2.0f + 1) * t + 2.0f) + 1.0f;
	}

	public static float AnticipateInterpolator(float t){
		return t * t * ((2.0f + 1) * t - 2.0f);
	}

	public void StartLevel(){
		if (SquareClick.windowOut && SquareClick.canMove) {
			print ("Level_m" + mondeActuel + "_" + SquareClick.selectedLvl);
			CheckBox checkEasy = GameObject.FindGameObjectWithTag ("ValidEasy").GetComponent<CheckBox> ();
			CheckBox checkCoop = GameObject.FindGameObjectWithTag("ValidCoop").GetComponent<CheckBox>();
			GameObject p = new GameObject ("Params");
			p.tag = "Params";
			DontDestroyOnLoad (p);
			ReferenceObj r = p.AddComponent<ReferenceObj> ();
			r.names = new string[]{"isEasy", "isCoop"};
			r.objects = new object[]{checkEasy.isChecked, checkCoop.isChecked};
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Level_m" + mondeActuel + "_" + SquareClick.selectedLvl);
		}
	}
}

public delegate float Interpolator(float input);
public delegate void OnAnimationUpdate(float value);
public delegate void OnAnimationUpdateObj(float value, object o);

public class SetColorSRGameObstacle : GameObstacle {
	private SpriteRenderer sr;

	public override Type GetPlayModeType (){
		return typeof(SetColorSRGameObstacle);
	}

	void Start(){
	}

	public override void SetupEditMode (){}

	public override void SetColor(Color color){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

	public override Color GetColor (){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		return sr.color;
	}
}