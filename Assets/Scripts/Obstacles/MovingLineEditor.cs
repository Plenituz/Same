using UnityEngine;
using System;

public class MovingLineEditor : GameObstacle {
	public float speed = 0f;
	public float size = 10;
	public float height = 1f;
	public float position = 0f;
	public START_AT startAtPos;
	public float lifeTime = -1f;
	public float dieAtPos = -50f;
	private SpriteRenderer sr;

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

	public override void SetupEditMode(){
		switch (startAtPos) {
		case START_AT.BOTTOM:
			transform.position = new Vector3 (P.pocP(position, Side.WIDTH), P.pocP (-1f - (size / 100), Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSEditor (height, Side.WIDTH), P.pocSEditor (size / 100, Side.HEIGHT), 1f);
			break;
		case START_AT.TOP:
			transform.position = new Vector3 (P.pocP(position, Side.WIDTH), P.pocP(1f + (size/100), Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSEditor(height, Side.WIDTH), P.pocSEditor(size/100, Side.HEIGHT), 1f);
			break;
		case START_AT.LEFT:
			transform.position = new Vector3 (P.pocP(-1f - (size/100), Side.WIDTH), P.pocP(position, Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSEditor(size/100, Side.WIDTH), P.pocSEditor(height, Side.HEIGHT), 1f);
			break;
		case START_AT.RIGHT:
			transform.position = new Vector3 (P.pocP(1f + (size/100), Side.WIDTH), P.pocP(position, Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSEditor(size/100, Side.WIDTH), P.pocSEditor(height, Side.HEIGHT), 1f);
			break;
		}
	}

	public override Type GetPlayModeType (){
		return typeof(MovingLine);
	}
}

public class MovingLine : MovingLineEditor {
	private float startTime;

	void Start(){
		startTime = Time.time;
		speed /= 100;
		switch (startAtPos) {
		case START_AT.BOTTOM:
			transform.position = new Vector3 (P.pocP(position, Side.WIDTH), P.pocP (-1f - (size / 100), Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSGame (height, Side.WIDTH), P.pocSGame (size / 100, Side.HEIGHT), 1f);
			break;
		case START_AT.TOP:
			speed *= -1;
			transform.position = new Vector3 (P.pocP(position, Side.WIDTH), P.pocP(1f + (size/100), Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSGame(height, Side.WIDTH), P.pocSGame(size/100, Side.HEIGHT), 1f);
			break;
		case START_AT.LEFT:
			transform.position = new Vector3 (P.pocP(-1f - (size/100), Side.WIDTH), P.pocP(position, Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSGame(size/100, Side.WIDTH), P.pocSGame(height, Side.HEIGHT), 1f);
			break;
		case START_AT.RIGHT:
			speed *= -1;
			transform.position = new Vector3 (P.pocP(1f + (size/100), Side.WIDTH), P.pocP(position, Side.HEIGHT), 0f);
			transform.localScale = new Vector3 (P.pocSGame(size/100, Side.WIDTH), P.pocSGame(height, Side.HEIGHT), 1f);
			break;
		}
	}

	void FixedUpdate(){
		switch (startAtPos) {
		case START_AT.TOP:
		case START_AT.BOTTOM:
			transform.position += new Vector3 (0f, speed, 0f);
			break;
		case START_AT.LEFT:
		case START_AT.RIGHT:
			transform.position += new Vector3 (speed, 0f, 0f);
			break;
		}
		if (lifeTime > 0 && Time.time - startTime >= lifeTime) {
			Destroy (gameObject);
		}
		if (dieAtPos != -50f) {
			switch (startAtPos) {
			case START_AT.BOTTOM:
				if (transform.position.y >= P.pocP (dieAtPos, Side.H)) {
					Destroy (gameObject);
				}
				break;
			case START_AT.TOP:
				if (transform.position.y <= P.pocP (dieAtPos, Side.H)) {
					Destroy (gameObject);
				}
				break;
			case START_AT.LEFT:
				if (transform.position.x >= P.pocP (dieAtPos, Side.W)) {
					Destroy (gameObject);
				}
				break;
			case START_AT.RIGHT:
				if (transform.position.x <= P.pocP (dieAtPos, Side.W)) {
					Destroy (gameObject);
				}
				break;
			}
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		if (collider.CompareTag ("Boundary"))
			Destroy (gameObject);
	}
}

public enum START_AT{
	TOP,
	BOTTOM,
	LEFT,
	RIGHT
}




