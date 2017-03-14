using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour, Multicolorable {

	/**
	 * Collison woth player has happened
	 */
	public static OnCollisionWith onCollision;
	/*
	 * Player is declared dead
	 */
	public static Runnable onDeath;
	/**
	 * Color switch is supposed to be taken care of in here
	 */
	public static Runnable onColorSwitchSignal;

	public static Runnable afterColorChange;
	/**
	 * Called just after the player moved
	 */
	public static DelegateGameObject onPlayerMove;
	/**
	 * Called OnRenderObject
	 */
	public static DelegateGameObject onRenderObject;

	public static void Reset(){
		onRenderObject = null;
		onPlayerMove = null;
		afterColorChange = null;
		onColorSwitchSignal = null;
		onDeath = null;
		onCollision = null;
	}

	protected MeshRenderer mr;
	private bool allowed = false;
	protected TrailEffect trailEffect = new ColorSwitchTrailEffect ();
	private GameStartup gameStartup;
	private Text lifeCountText;
	private int lifeCount = 3;

	public virtual void SetColor(Color color){
		mr.material.color = color;
		if (afterColorChange != null) {
			afterColorChange ();
		}
	}

	public virtual Color GetColor(){
		return mr.material.color;
	}

	public virtual void Start(){
		mr = GetComponent<MeshRenderer> ();
		mr.material = new Material (Shader.Find ("Sprites/Default"));
		onCollision += OnCollisionDefault;
		onDeath += OnDeathDefault;
		onColorSwitchSignal += OnColorSwitchSignalDefault;
		trailEffect.Create (gameObject);
	}

	public virtual void Update(){
		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch (0);
			Vector2 posTouch = new Vector2 (P.pocP ( ((t.position.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((t.position.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
			switch (t.phase) {
			case TouchPhase.Began:
				if (Vector2.Distance(posTouch, transform.position) <= 0.5f) {
					allowed = true;
				} else {
					allowed = false;
					if (onColorSwitchSignal != null)
						onColorSwitchSignal ();
				}
				break;
			case TouchPhase.Moved:
				if (allowed) {
					transform.position = posTouch;
					if (onPlayerMove != null)
						onPlayerMove (gameObject);
				}
				break;
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				allowed = false;
				break;
			}
		}
		if (allowed && Input.touchCount > 1 && Input.touches[1].phase == TouchPhase.Began) {
			if (onColorSwitchSignal != null)
				onColorSwitchSignal ();
		}
		#if UNITY_EDITOR
		if(Input.GetMouseButton(0)){
			Vector2 posTouch = new Vector2 (P.pocP ( ((Input.mousePosition.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((Input.mousePosition.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
			transform.position = posTouch;
			if (onPlayerMove != null)
				onPlayerMove (gameObject);
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			if(onColorSwitchSignal != null)
				onColorSwitchSignal();
		}
		#endif
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(onCollision != null)
			onCollision (collider);
	}

	void OnRenderObject(){
		if (onRenderObject != null)
			onRenderObject (gameObject);
	}

	public void OnCollisionDefault(Collider2D other){
		if (other.CompareTag ("Obstacle")) {
			GameObstacle go = other.gameObject.GetComponent<GameObstacle> ();
			if (go.GetColor () != mr.material.color) {
				if (onDeath != null)
					onDeath ();
			}
		}
	}

	public void OnColorSwitchSignalDefault(){
		SetColor (mr.material.color == Color.black ? Color.white : Color.black);
	}

	public void OnDeathDefault(){
		if (GetType () == typeof(PlayerScript)) {
			GameObject explPrefab = Resources.Load<GameObject> ("PowerIcons/ExploDeath");
			GameObject g = Instantiate (explPrefab) as GameObject;
			g.transform.position = transform.position;
		}

		if(gameStartup == null)
			gameStartup = Camera.main.GetComponent<GameStartup> ();
		if (gameStartup.easyMode) {
			if (lifeCountText == null) {
				lifeCountText = GameObject.FindGameObjectWithTag ("PowerUI").transform.FindChild ("LifeCount").GetComponent<Text> ();
			}
			lifeCount--;
			lifeCountText.text = lifeCount.ToString ();
			if (lifeCount <= 0)
				EndLevel ();
		} else {
			EndLevel ();
		}
	}

	public void EndLevel(){
		LevelEndScript.CreateEndLevelObj (UnityEngine.SceneManagement.SceneManager.GetActiveScene
			().name, Completion.DIED);
		LevelEndScript.SpawnEndAnim (Color.black);
		StartCoroutine(WorldSelectBanner.AnimateValue(0f, 1f, 0.8f, null, 
			(float value) => {
				if(value == 1f)
					UnityEngine.SceneManagement.SceneManager.LoadScene ("DeathScreen");
			}));
	}
}

public delegate void OnCollisionWith(Collider2D other);
public delegate void Runnable();
public delegate void DelegateGameObject(GameObject g);
