using System;
using UnityEngine;

public class ParticleEditor : GameObstacle{
	[HideInInspector] public Material mat;
	[HideInInspector] public GameObject handle1;
	[HideInInspector] public GameObject handle2;
	[HideInInspector] public GameObject endPoint;

	public float dieWhen = 5f;
	public Color color = Color.white;
	public Vector2 position;
	public float size = 10f;
	public float thickness = 1f;
	public bool isTriangle;
	public float triangleRotation;
	public MovementType movementType;
	public float angle;
	public float speed = 10f;

	public override void SetColor (Color color){
		this.color = color;
	}

	public override Color GetColor (){
		return color;
	}

	public override Type GetPlayModeType (){
		return typeof(Particle);
	}

	public override void SetupEditMode (){
		if (handle1 == null) {
			handle1 = transform.FindChild ("Handle1").gameObject;
			handle2 = transform.FindChild ("Handle2").gameObject;
			endPoint = transform.FindChild ("EndPoint").gameObject;
		}

		if (triangleRotation < 0)
			triangleRotation = 0;
		if (size < 0)
			size = 0;

		if(mat == null)
			mat = new Material (Shader.Find ("Sprites/Default"));

		if (movementType != MovementType.PATH) {
			handle1.SetActive(false);
			handle2.SetActive(false);
			endPoint.SetActive(false);
		}

	}

	void OnRenderObject(){
		Render ();
		#if UNITY_EDITOR
		if(UnityEditor.Selection.Contains(handle1) || UnityEditor.Selection.Contains(handle2) || UnityEditor.Selection.Contains(endPoint) || UnityEditor.Selection.Contains(gameObject)){
			if (movementType == MovementType.PATH){
				RenderPath ();
				handle1.SetActive(true);
				handle2.SetActive(true);
				endPoint.SetActive(true);
			}else if(movementType == MovementType.VELOCITY_VECTOR){
				Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;
				ComplexNumber c = new ComplexNumber (speed, angle * Mathf.Deg2Rad, ComplexNumber.GEOMETRICAL);
				Debug.DrawLine(position, position + new Vector2 (P.pocP(c.getRealPart(), Side.W), P.pocP(c.getImaginaryPart(), Side.H))/10, Color.red, 5f);
			}
		}else{
			handle1.SetActive(false);
			handle2.SetActive(false);
			endPoint.SetActive(false);
		}
		#endif
	}

	void RenderPath (){
		if (this.handle1 == null)
			return;
		
		Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;
		Vector2 handle1Pos = new Vector2 (P.pocP (handle1.transform.position.x, Side.WIDTH), P.pocP (handle1.transform.position.y, Side.HEIGHT)) / 10;
		Vector2 handle2Pos = new Vector2 (P.pocP (handle2.transform.position.x, Side.WIDTH), P.pocP (handle2.transform.position.y, Side.HEIGHT)) / 10;
		Vector2 endPointPos = new Vector2 (P.pocP (endPoint.transform.position.x, Side.WIDTH), P.pocP (endPoint.transform.position.y, Side.HEIGHT)) / 10;

		Path path = new Path (position);
		path.CubicTo (new Vector2 (handle1Pos.x, handle1Pos.y), new Vector2 (handle2Pos.x, handle2Pos.y), new Vector2 (endPointPos.x, endPointPos.y));
		path.Seal ();
		float step = 0.02f;
		for (float i = 0; i < 1f - step; i += step) {
			Vector2 p1 = path.GetPointAtPercent (i);
			Vector2 p2 = path.GetPointAtPercent (i + step);
			Debug.DrawLine (p1, p2, Color.red, 10f);
		}
	}

	public void Render(){
		float step = 0.02f;
		Vector3[] quad = null;
		Path path = isTriangle ? TrianglePath () : CirclePath ();

		GL.PushMatrix ();
		mat.SetPass (0);
		GL.MultMatrix (transform.localToWorldMatrix);
		GL.Begin (GL.QUADS);
		GL.Color (color);

		for (float i = 0; i < 1f; i += step) {
			Vector2 p1 = path.GetPointAtPercent (i);
			Vector2 p2 = path.GetPointAtPercent (i + step);
			if (quad == null) {
				quad = MakeQuad (p1, p2, thickness / 10);
				foreach (Vector3 v in quad) {
					GL.Vertex (v);
				}
			} else {
				Vector3[] newquad = MakeQuad (p1, p2, thickness / 10);
				GL.Vertex (quad [1]);
				GL.Vertex (newquad [1]);
				GL.Vertex (newquad [2]);
				GL.Vertex (quad [2]);
				quad = newquad;
			}
		}
		GL.End ();
		GL.PopMatrix ();
	}

	public Path CirclePath(){
		Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;
		float size = P.pocP (this.size / 100, Side.WIDTH);
		Path path = new Path (position + new Vector2 (0f, -size));

		path.CubicTo (position + new Vector2 (size * 0.545f, -size), 
			position + new Vector2 (size, -size * 0.545f), 
			position + new Vector2 (size, 0f));

		path.CubicTo (position + new Vector2 (size, size * 0.545f), 
			position + new Vector2 (size * 0.545f, size), 
			position + new Vector2 (0f, size));

		path.CubicTo (position + new Vector2 (-size * 0.545f, size), 
			position + new Vector2 (-size, size * 0.545f), 
			position + new Vector2 (-size, 0f));

		path.CubicTo (position + new Vector2 (-size, -size * 0.545f), 
			position + new Vector2 (-size * 0.545f, -size), 
			position + new Vector2 (0f, -size));

		path.Seal ();
		return path;
	}

	public Path TrianglePath(){
		Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;
		float size = P.pocP (this.size / 100, Side.W);
		Path path = new Path (ComplexNumber.rotate(position, position + new Vector2 (0f, size), triangleRotation));
		path.CubicTo (
			ComplexNumber.rotate(position, position + new Vector2 (0f, size), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (0.87f * size, -size / 2), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (0.87f * size, -size / 2), triangleRotation)
		);

		path.CubicTo (
			ComplexNumber.rotate(position, position + new Vector2 (0.87f * size, -size / 2), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (-0.87f * size, -size / 2), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (-0.87f * size, -size / 2), triangleRotation)
		);

		path.CubicTo (
			ComplexNumber.rotate(position, position + new Vector2 (-0.87f * size, -size / 2), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (0f, size), triangleRotation), 
			ComplexNumber.rotate(position, position + new Vector2 (0f, size), triangleRotation)
		);

		path.Seal ();
		return path;
	}

	public Vector3[] MakeQuad(Vector3 s, Vector3 e, float w) {
		w = w / 2;
		Vector3[] q = new Vector3[4];

		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();

		q[0] = transform.InverseTransformPoint(s + l * w);
		q[3] = transform.InverseTransformPoint(s + l * -w);
		q[1] = transform.InverseTransformPoint(e + l * w);
		q[2] = transform.InverseTransformPoint(e + l * -w);

		return q;
	}
}

public class Particle : ParticleEditor {
	private Path path;
	private GameObject player;
	private CircleCollider2D collCircle;
	private EdgeCollider2D collEdge;
	private Vector2 towardPlayerPoc;
	private float startTime;
	private float pathPercent = 0f;
	private float oldAngle = 0f;
	private int skip = -1;


	void Start(){
		startTime = Time.time;
		speed /= 100;
		mat = new Material (Shader.Find ("Sprites/Default"));
		//position = new Vector2 (P.pocP (position.x, Side.WIDTH), P.pocP (position.y, Side.HEIGHT)) / 10;
		if (isTriangle) {
			collEdge = gameObject.AddComponent<EdgeCollider2D> ();
			collEdge.isTrigger = true;
			float size = P.pocP (this.size / 100, Side.W);
			size += thickness / 10;
			//Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;
			Vector2[] ps = new Vector2[4];
			ps [0] = ComplexNumber.rotate (Vector2.zero, new Vector2 (0f, size), triangleRotation);
			ps [1] = ComplexNumber.rotate (Vector2.zero, new Vector2 (0.87f * size, -size / 2), triangleRotation);
			ps [2] = ComplexNumber.rotate (Vector2.zero, new Vector2 (-0.87f * size, -size / 2), triangleRotation);
			ps [3] = ps [0];
			collEdge.points = ps;
		} else {
			collCircle = gameObject.AddComponent<CircleCollider2D> ();
			collCircle.radius = size * 0.035f + thickness* 0.04f;
			collCircle.offset = position;
			collCircle.isTrigger = true;
		}
		path = new Path (this.position);
		//handle1.transform.position = new Vector2 (P.pocP (handle1.transform.position.x, Side.WIDTH), P.pocP (handle1.transform.position.y, Side.HEIGHT)) / 10;
	//	handle2.transform.position = new Vector2 (P.pocP (handle2.transform.position.x, Side.WIDTH), P.pocP (handle2.transform.position.y, Side.HEIGHT)) / 10;
	//	endPoint.transform.position = new Vector2 (P.pocP (endPoint.transform.position.x, Side.WIDTH), P.pocP (endPoint.transform.position.y, Side.HEIGHT)) / 10;
		path.CubicTo (handle1.transform.position, handle2.transform.position, endPoint.transform.position);
		path.Seal ();
		Destroy (handle1);
		Destroy (handle2);
		Destroy (endPoint);
		if (movementType == MovementType.FOLLOW_PLAYER) {
			player = GameObject.FindGameObjectWithTag ("Player");
			//angle /= 10;
		}
	}

	public void RenderAndUpdateCollider(){
		Vector2 position = new Vector2 (P.pocP (this.position.x, Side.WIDTH), P.pocP (this.position.y, Side.HEIGHT)) / 10;

		if (isTriangle)
			collEdge.offset = position;
		else
			collCircle.offset = position;
		
		float step = 0.02f;
		Vector3[] quad = null;
		Path path = isTriangle ? TrianglePath () : CirclePath ();

		GL.PushMatrix ();
		mat.SetPass (0);
		GL.MultMatrix (transform.localToWorldMatrix);
		GL.Begin (GL.QUADS);
		GL.Color (color);

		for (float i = 0; i < 1f; i += step) {
			Vector2 p1 = path.GetPointAtPercent (i);
			Vector2 p2 = path.GetPointAtPercent (i + step);

			if (quad == null) {
				quad = MakeQuad (p1, p2, thickness / 10);
				foreach (Vector3 v in quad) {
					GL.Vertex (v);
				}
			} else {
				Vector3[] newquad = MakeQuad (p1, p2, thickness / 10);
				GL.Vertex (quad [1]);
				GL.Vertex (newquad [1]);
				GL.Vertex (newquad [2]);
				GL.Vertex (quad [2]);
				quad = newquad;
			}
		}
		GL.End ();
		GL.PopMatrix ();
	}

	void FixedUpdate(){
		switch (movementType) {
		case MovementType.VELOCITY_VECTOR:
			ComplexNumber c = new ComplexNumber (speed, angle * Mathf.Deg2Rad, ComplexNumber.GEOMETRICAL);
			position += new Vector2 (c.getRealPart (), c.getImaginaryPart ());
			if (Time.time - startTime >= dieWhen) {
				Destroy (gameObject);
			}
			break;
		case MovementType.PATH:
			position = path.GetPointAtPercent ((pathPercent += speed) / 10);
			if (pathPercent/10 >= 1f)
				Destroy (gameObject);
			break;
		case MovementType.FOLLOW_PLAYER:

			if (skip++ >= angle || skip == -1) {
				skip = 0;
				Vector2 positionWorld = new Vector2 (P.pocP (position.x, Side.W), P.pocP (position.y, Side.H)) / 10f;
				ComplexNumber convert = new ComplexNumber (player.transform.position.x - positionWorld.x, player.transform.position.y - positionWorld.y, ComplexNumber.NUMERICAL);
				ComplexNumber towardPlayerWorldComplex;

				towardPlayerWorldComplex = new ComplexNumber (speed, convert.getArgument (), ComplexNumber.GEOMETRICAL);

				towardPlayerPoc = new Vector2 (P.unPocP (towardPlayerWorldComplex.getRealPart () * 10, Side.W), P.unPocP (towardPlayerWorldComplex.getImaginaryPart () * 10, Side.H));
				//la positione est en % *10 [-10; 10]
				//Debug.DrawLine(positionWorld, new Vector2(towardPlayerWorldComplex.getRealPart(), towardPlayerWorldComplex.getImaginaryPart()), Color.red);

				oldAngle = towardPlayerWorldComplex.getArgument ();
			}
			position += towardPlayerPoc;

			#if false
			Vector2 positionWorld = new Vector2 (P.pocP (position.x, Side.W), P.pocP (position.y, Side.H)) / 10f;
			ComplexNumber convert = new ComplexNumber (player.transform.position.x - positionWorld.x, player.transform.position.y - positionWorld.y, ComplexNumber.NUMERICAL);
			ComplexNumber towardPlayerWorldComplex;

			//print (Mathf.Abs(oldAngle - convert.getArgument()) >= angle);
			towardPlayerWorldComplex = new ComplexNumber (speed, Mathf.Clamp(convert.getArgument(), oldAngle - (angle*Mathf.Deg2Rad), oldAngle + (angle*Mathf.Deg2Rad)), ComplexNumber.GEOMETRICAL);

			Vector2 towardPlayerPoc = new Vector2 (P.unPocP (towardPlayerWorldComplex.getRealPart ()*10, Side.W), P.unPocP (towardPlayerWorldComplex.getImaginaryPart ()*10, Side.H));
			//la positione est en % *10 [-10; 10]
			//Debug.DrawLine(positionWorld, new Vector2(towardPlayerWorldComplex.getRealPart(), towardPlayerWorldComplex.getImaginaryPart()), Color.red);

			oldAngle = towardPlayerWorldComplex.getArgument();
			position += towardPlayerPoc;
			#endif
			if (Time.time - startTime >= dieWhen) {
				Destroy (gameObject);
			}
			break;
		}
	}

	void OnRenderObject(){
		RenderAndUpdateCollider ();
	}
}

public enum MovementType{
	VELOCITY_VECTOR,
	PATH,
	FOLLOW_PLAYER
}


