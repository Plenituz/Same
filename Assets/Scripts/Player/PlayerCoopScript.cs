using System;
using UnityEngine;

public class PlayerCoopScript : PlayerScript {
	private PlayerCoopScript otherPlayer;
	private bool isMaster;
	public int fingerId = -1;
	private bool dragging = false;
	private bool switched = false;
	private int fingerSwitchId = -1;

	public override void SetColor (Color color){
		if (isMaster) {
			otherPlayer.SetColorSlave (color);
			SetColorSlave (color);
			if (afterColorChange != null) 
				afterColorChange ();
		} else {
			otherPlayer.SetColor (color);
		}
	}

	public void SetColorSlave(Color color){
		mr.material.color = color;
	}

	public override Color GetColor (){
		if (isMaster)
			return mr.material.color;
		else
			return otherPlayer.GetColor ();
	}

	public override void Start (){
		mr = GetComponent<MeshRenderer> ();
		mr.material = new Material (Shader.Find ("Sprites/Default"));
		//onCollision += OnCollisionDefault;
		trailEffect.Create (gameObject);
		if (isMaster) {
			onColorSwitchSignal += OnColorSwitchSignalDefault;
			onDeath += OnDeathDefault;
		}
	}

	public override void Update (){
		if (Input.touchCount > 0) {
			if (fingerId != -1) {
				//Vector2 posTouch = new Vector2 (P.pocP ( ((t.position.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((t.position.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
				//Vector2.Distance(posTouch, transform.position) <= 0.5f
				Touch t = default(Touch);

				for (int i = 0; i < Input.touchCount; i++) {
					if (Input.touches [i].fingerId == fingerId) {
						t = Input.touches [i];
					}
				}
				if (t.Equals(default(Touch))) {
					fingerId = -1;
					dragging = false;
					return;
				}
				Vector2 posTouch = new Vector2 (P.pocP ( ((t.position.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((t.position.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
				switch (t.phase) {
				case TouchPhase.Began:
					if (Vector2.Distance (posTouch, transform.position) <= 0.5f) {
						dragging = true;
					}
					break;
				case TouchPhase.Moved:
					if (dragging) {
						transform.position = posTouch;
						if (onPlayerMove != null)
							onPlayerMove (gameObject);
					}
					break;
				case TouchPhase.Canceled:
				case TouchPhase.Ended:
					dragging = false;
					fingerId = -1;
					break;
				}
			} else {
				for (int i = 0; i < Input.touchCount; i++) {
					Touch t = Input.GetTouch (i);
					Vector2 posTouch = new Vector2 (P.pocP ( ((t.position.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((t.position.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
					if (t.fingerId != otherPlayer.fingerId && Vector2.Distance (posTouch, transform.position) <= 0.5f) {
						fingerId = t.fingerId;
						Update ();
						return;
					}
				}
			}
			if (isMaster) {
				for (int i = 0; i < Input.touchCount; i++) {
					Touch t = Input.GetTouch (i);
					Vector2 posTouch = new Vector2 (P.pocP ( ((t.position.x / Screen.width) - 0.5f)*2f, Side.WIDTH), P.pocP (((t.position.y / Screen.height) - 0.5f)*2f, Side.HEIGHT));
					if (!(Vector2.Distance (posTouch, transform.position) <= 0.5f || Vector2.Distance (posTouch, otherPlayer.transform.position) <= 0.5f) && !switched) {
						if (t.fingerId != otherPlayer.fingerId || t.fingerId != fingerId) {
							switched = true;
							fingerSwitchId = t.fingerId;
							if (onColorSwitchSignal != null)
								onColorSwitchSignal ();
						}
					} else if(t.fingerId == fingerSwitchId && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)) {
						switched = false;
						fingerSwitchId = -1;
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if (collider.CompareTag ("Obstacle")) {
			GameObstacle go = collider.gameObject.GetComponent<GameObstacle> ();
			if (go.GetColor () != mr.material.color) {
				GameObject explPrefab = Resources.Load<GameObject> ("PowerIcons/ExploDeath");
				if (onDeath != null) {
					GameObject g = Instantiate (explPrefab) as GameObject;
					g.transform.position = transform.position;
					onDeath ();
				}
			}
		}
	}

	public static void LinkPlayers(PlayerCoopScript player1, PlayerCoopScript player2){
		player1.otherPlayer = player2;
		player2.otherPlayer = player1;
		player1.isMaster = true;
	}
}


