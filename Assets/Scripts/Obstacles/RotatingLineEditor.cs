using System;
using UnityEngine;

public class RotatingLineEditor : GameObstacle{
	public float speed = 1f;
	public Vector2 sizes = new Vector2(4f, 60f);
	public Vector2 startAtPos;
	public float startAngle;
	public float lifeTime = 5f;
	private SpriteRenderer sr;

	public override void SetColor(Color color){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

	public override Color GetColor ()
	{
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		return sr.color;
	}

	public override void SetupEditMode(){
		transform.position = new Vector2 (P.pocP (startAtPos.x, Side.WIDTH), P.pocP (startAtPos.y, Side.HEIGHT))/10;
		transform.rotation = Quaternion.Euler (0f, 0f, startAngle);
		transform.localScale = new Vector3 (P.pocSEditor (sizes.x/100, Side.WIDTH), P.pocSEditor (sizes.y/100, Side.HEIGHT), 1f);
	}

	public override Type GetPlayModeType (){
		return typeof(RotatingLine);
	}
}

public class RotatingLine : RotatingLineEditor{
	private float startTime;

	void Start(){
		startTime = Time.time;
		speed /= 100;
		transform.position = new Vector2 (P.pocP (startAtPos.x, Side.WIDTH), P.pocP (startAtPos.y, Side.HEIGHT))/10;
		transform.rotation = Quaternion.Euler (0f, 0f, startAngle);
		transform.localScale = new Vector3 (P.pocSGame (sizes.x/100, Side.WIDTH), P.pocSGame (sizes.y/100, Side.HEIGHT), 1f);
	}

	void FixedUpdate(){
		transform.rotation = Quaternion.Euler (0f, 0f, transform.rotation.eulerAngles.z + speed);
		if (Time.time - startTime >= lifeTime) {
			Destroy (gameObject);
		}
	}
}


