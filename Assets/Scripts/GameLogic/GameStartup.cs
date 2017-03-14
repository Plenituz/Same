using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class GameStartup : MonoBehaviour {
	public static Runnable onUpdate;

	public GameObject playerPrefab;
	public ArrayList obstacles = new ArrayList();
	[HideInInspector] public float startTime;
	public float timeOffset = 0f;
	[HideInInspector] public bool easyMode = false;
	[HideInInspector] public bool coopMode = false;

	private Material mat;
	private Color contourColor;
	private PlayerScript playerInstance;


	void Awake () {
		P.init ();//init the magic number for screen scaling
		Time.timeScale = 1f; //just in case it had been changed
		WorldSelectBanner.Reset ();
		SquareClick.Reset ();

		//create the boundary at the screen's size
		GameObject boundary = new GameObject ();
		boundary.name = "Boundary";
		boundary.tag = "Boundary";
		BoxCollider2D boundaryColl = boundary.AddComponent<BoxCollider2D> ();
		boundaryColl.isTrigger = true;
		boundaryColl.size = new Vector2 (P.pocP (2f, Side.W), P.pocP (2f, Side.H));

		//background
		GameObject bg = new GameObject("Background");
		bg.transform.position = Vector3.zero;
		bg.transform.localScale = new Vector3 (P.pocSGame (1f, Side.W), P.pocSGame (1f, Side.H), 1f);
		SpriteRenderer srBg = bg.AddComponent<SpriteRenderer> ();
		srBg.sortingOrder = -100;
		srBg.sprite = Resources.Load<Sprite> ("pixel");
		//Level_mX_XX
		try{
			switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.ToCharArray () [7]) {
			case '1':
				srBg.color = new Color (0.22352941176470588235294117647059f, 0.32549019607843137254901960784314f, 0.64313725490196078431372549019608f, 1f);
				break;
			case '2':
				srBg.color = new Color (0.64705882352941176470588235294118f, 0.21568627450980392156862745098039f, 0.21568627450980392156862745098039f, 1f);
				break;
			case '3':
				srBg.color = new Color (0.23529411764705882352941176470588f, 0.18039215686274509803921568627451f, 0.11372549019607843137254901960784f, 1f);
				break;
			default:
				srBg.color = new Color (1f, 0f, 0f, 1f);
				break;
			}
		}catch(Exception){
			srBg.color = new Color (1f, 0f, 0f, 1f);
		}

		//params
		GameObject para = GameObject.FindGameObjectWithTag("Params");
		if (para != null) {
			ReferenceObj refe = para.GetComponent<ReferenceObj> ();
			easyMode = (bool)refe.GetReferenceByName ("isEasy");
			coopMode = (bool)refe.GetReferenceByName ("isCoop");
			Destroy (para);
		}


		//player
		if (coopMode) {
			GameObject player1 = Instantiate (playerPrefab) as GameObject;
			GameObject player2 = Instantiate (playerPrefab) as GameObject;
			player1.name = "player1";
			player2.name = "player2";
		//	Destroy (player1.GetComponent<PlayerScript> ());
		//	Destroy (player2.GetComponent<PlayerScript> ());
			PlayerCoopScript pScript1 = player1.AddComponent<PlayerCoopScript> ();
			PlayerCoopScript pScript2 = player2.AddComponent<PlayerCoopScript> ();
			PlayerCoopScript.LinkPlayers (pScript1, pScript2);

			player1.transform.position = new Vector2 (0, P.pocP (-0.7f, Side.HEIGHT));
			player2.transform.position = new Vector2 (0, P.pocP (0.7f, Side.HEIGHT));
			mat = player1.GetComponent<MeshRenderer> ().material;
			contourColor = mat.color;
			playerInstance = pScript1;
		} else {
			GameObject player = Instantiate(playerPrefab) as GameObject;
			player.transform.position = new Vector2 (0, P.pocP (-0.7f, Side.HEIGHT));
			mat = player.GetComponent<MeshRenderer> ().material;
			playerInstance = player.AddComponent<PlayerScript> ();
			contourColor = mat.color;
		}
		PlayerScript.afterColorChange += OnColorChange;

		//obstacles
		GameObject[] obs = GameObject.FindGameObjectsWithTag ("Obstacle");
		obstacles.AddRange (obs);
		StartCoroutine (Enabler());
		startTime = Time.time;

		foreach (object g in obstacles) {
			((GameObject)g).SetActive (false);
			if (((GameObject)g).GetComponent<GameObstacle> ().startAtTime < timeOffset) {
				Destroy ((GameObject) g);
			}
		}

		//setup UI
		Data data = Data.GetInstance();
		GameObject ui = Instantiate (Resources.Load<GameObject> ("PowerIcons/PowerButtons")) as GameObject;
		if (easyMode) {
			ui.transform.FindChild ("LifeCount").gameObject.SetActive (true);
		}
		for (int i = 0; i < 4; i++) {
			GameObject powUi = ui.transform.FindChild ("Power" + (i+1)).gameObject;
			PowerScript powScript = powUi.GetComponent<PowerScript> ();
			Slider slider = powUi.GetComponent<Slider> ();
			PowerAttribut attrib = data.GetPowerAttribut (data.selectedPowers [i]);
			Power pow = Power.GetInstance (data.selectedPowers [i], slider);
			if (data.selectedPowers [i] == Powers.Null) {
				powUi.SetActive (false);
			} else {
				pow.SetAttribut (attrib);
				powScript.power = pow;
				powUi.transform.FindChild ("Icon").GetComponent<Image> ().sprite = pow.icon;
			}
		}
	}

	void OnColorChange(){
		contourColor = playerInstance.GetColor ();
	}

	IEnumerator Enabler(){
		while (true) {
			for (int i = 0; i < obstacles.Count; i++) {
				GameObject g = ((GameObject)obstacles [i]);
				if (g == null)
					continue;
				if (!g.activeSelf && Time.time - startTime >= g.GetComponent<GameObstacle> ().startAtTime - timeOffset) {
					g.SetActive (true);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	void Update(){
		if (onUpdate != null)
			onUpdate ();
	}

	void OnRenderObject(){
		GL.PushMatrix ();
		GL.MultMatrix (transform.worldToLocalMatrix);
		mat.SetPass (0);
		GL.Begin (GL.QUADS);
		GL.Color (contourColor);
		// left
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (-0.95f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (-0.95f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		//bottom
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT) + P.pocP(0.05f, Side.W), 0f);
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT) + P.pocP(0.05f, Side.W), 0f);
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		//right
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (0.95f, Side.WIDTH), P.pocP (-1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (0.95f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		//top
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		GL.Vertex3 (P.pocP (1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT) - P.pocP(0.05f, Side.W), 0f);
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT) - P.pocP(0.05f, Side.W), 0f);
		GL.Vertex3 (P.pocP (-1f, Side.WIDTH), P.pocP (1f, Side.HEIGHT), 0f);
		GL.End ();
		GL.PopMatrix ();
	}
}
