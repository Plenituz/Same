using System;
using System.Collections;
using UnityEngine;

public class SizeChangerEditor : GameObstacle {
	[HideInInspector] public Material mat;
	public float dieWhen = -1;
	public Color color;
	public Vector2 position;
	public float size;
	public float thickness;
	public float speed = 10f;
	public bool isTriangle;
	public float triangleRotation;

	public override void SetColor(Color color){
		this.color = color;
	}

	public override Color GetColor(){
		return color;
	}

	public override void SetupEditMode(){
		if (triangleRotation < 0)
			triangleRotation = 0;
		if (size < 0)
			size = 0;
		if(mat == null)
			mat = new Material (Shader.Find ("Sprites/Default"));
		mat.color = color;

	}

	void OnRenderObject(){
		RenderCircle ();
	}

	public override Type GetPlayModeType (){
		return typeof(SizeChanger);
	}

	public void RenderCircle(){
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

public class SizeChanger : SizeChangerEditor {
	private EdgeCollider2D edgeCollider1;
	private EdgeCollider2D edgeCollider2;
	private float startTime;
	private Vector2[] colliderPoints1;
	private Vector2[] colliderPoints2;

	void Start(){
		startTime = Time.time;
		edgeCollider1 = gameObject.AddComponent<EdgeCollider2D> ();
		edgeCollider1.isTrigger = true;
		edgeCollider2 = gameObject.AddComponent<EdgeCollider2D> ();
		edgeCollider2.isTrigger = true;

		speed /= 100;
		mat = new Material (Shader.Find ("Sprites/Default"));
	}

	new public void RenderCircle(){
		float step = 0.01f;
		int count = 0;
		int tmpCount = 0;
		Vector3[] quad = null;
		Path path = isTriangle ? TrianglePath () : CirclePath ();
		colliderPoints1 = new Vector2[(int)(1 / step + 1)/4];
		colliderPoints2 = new Vector2[(int)(1 / step + 1)/4];

		GL.PushMatrix ();
		mat.SetPass (0);
		GL.MultMatrix (transform.worldToLocalMatrix);
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

			if (tmpCount++ == 3) {
				tmpCount = 0;
				colliderPoints1 [count] = quad [1];
				colliderPoints2 [count++] = quad [2];
			}
		}

		edgeCollider1.points = colliderPoints1;
		edgeCollider2.points = colliderPoints2;
		GL.End ();
		GL.PopMatrix ();
	}
		
	void FixedUpdate(){
		if (size <= 1 && dieWhen < 0) {
			Destroy (gameObject);
		}
		if (dieWhen > 0) {
			if(Time.time - startTime >= dieWhen)
				Destroy(gameObject);
		}else if (size <= 0) {
			Destroy (gameObject);
		}
		size -= speed;
	}

	void OnRenderObject(){
		RenderCircle ();
	}
}


