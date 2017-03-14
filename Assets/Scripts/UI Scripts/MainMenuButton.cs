using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class MainMenuButton : GameObstacle {
	public float width = 1.0f;
	public Vector2 position;
	public bool move = true;
	public Text optionsText;
	public Text creditsText;
	[HideInInspector] public SpriteRenderer sr;

	public override Type GetPlayModeType (){
		return typeof(MainMenuButtonGame);
	}

	public override void SetupEditMode (){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();

		transform.localScale = new Vector3 (P.pocSEditor (width, Side.WIDTH), P.pocSEditor (0.08f, Side.HEIGHT), 1f);
		transform.position = new Vector3 (P.pocP (position.x, Side.WIDTH), P.pocP (position.y, Side.HEIGHT), 0f);
	}

	public override void SetColor(Color color){
		switch (gameObject.name) {
		case "Credits":
			creditsText.color = color == Color.black ? Color.white : Color.black;
			break;
		case "Options":
			optionsText.color = color == Color.black ? Color.white : Color.black;
			break;
		}
		sr.color = color;
	}

	public override Color GetColor (){
		return sr.color;
	}
}

public class MainMenuButtonGame : MainMenuButton{
	public float speed;
	private BoxCollider2D coll;
	private bool ok = false;

	void Awake(){
		if (!P.isInit)
			P.init ();
	}
	
	void Start () {
		coll = GetComponent<BoxCollider2D> ();
		if (move) {
			speed = UnityEngine.Random.Range (1f, 3f) / 100f;
			speed *= UnityEngine.Random.Range (0, 2) == 0 ? -1 : 1;
		}else
			speed = 0f;
		transform.localScale = new Vector3 (P.pocSGame (width, Side.WIDTH), P.pocSGame (0.08f, Side.HEIGHT), 1f);
		transform.position = new Vector3 (P.pocP (position.x, Side.WIDTH), P.pocP (position.y, Side.HEIGHT), 0f);
	}

	void FixedUpdate(){
		transform.position += new Vector3 (0f, speed, 0f);
	}

	void OnTriggerEnter2D(Collider2D collider){
		speed *= -1;
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
		switch (gameObject.name) {
		case "StoryButton":
			UnityEngine.SceneManagement.SceneManager.LoadScene ("WorldSelect");
			break;
		case "InfiniButton":

			break;
		case "ShopButton":
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Shop");
			break;
		case "Options":

			break;
		case "Credits":

			break;
		}
	}
}
