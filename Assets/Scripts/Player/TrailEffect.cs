using System;
using System.Reflection;
using UnityEngine;

public abstract class TrailEffect {
	public GameObject g;
	public abstract void Create (GameObject player);

	public void CleanTrailRenderers(GameObject player){
		TrailRenderer[] ts = player.GetComponents<TrailRenderer> ();
		foreach (TrailRenderer t in ts) {
			GameObject.Destroy (t);
		}
	}
}

public class BasicWhiteTrailEffect : TrailEffect {
	
	public override void Create (GameObject player){
		CleanTrailRenderers (player);
		TrailRenderer trail = player.AddComponent<TrailRenderer> ();
		trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		trail.receiveShadows = false;
		trail.material = new Material (Shader.Find ("Sprites/Default"));
		trail.startWidth = 0.66f;
		trail.endWidth = 0f;
		trail.time = 0.5f;
		trail.sortingOrder = -1;
	}
}

public class BasicBlackTrailEffect : TrailEffect {

	public override void Create (GameObject player){
		CleanTrailRenderers (player);
		TrailRenderer trail = player.AddComponent<TrailRenderer> ();
		trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		trail.receiveShadows = false;
		trail.material = new Material (Shader.Find ("Sprites/Default"));
		trail.startWidth = 0.66f;
		trail.endWidth = 0f;
		trail.time = 0.5f;
		trail.material.color = Color.black;
		trail.sortingOrder = -1;
	}
}

public class MulticolorTrailEffect : TrailEffect {

	public override void Create (GameObject player){
		GameObject g = Resources.Load<GameObject> ("TrailEffects/MulticolorTrailEffect");

		GameObject child = GameObject.Instantiate (g) as GameObject;
		child.transform.parent = player.transform;
		child.transform.position = player.transform.position;
		child.GetComponent<TrailRenderer> ().sortingOrder = -1;
	}
}

public class OldSchoolTrailEffect : TrailEffect {
	private Material mat;

	public override void Create (GameObject player){
		PlayerScript.onRenderObject += Trail;
		mat = new Material (Shader.Find ("Sprites/Default"));
	}

	void Trail(GameObject player){
		DrawGLCircle (player.transform.position, Color.red, P.pocP(0.1f, Side.W), P.pocP(0.01f, Side.W));
	}

	void DrawGLCircle(Vector2 pos, Color color, float size, float thickness){
		GL.PushMatrix ();
		mat.SetPass (0);
		GL.Begin (GL.QUADS);
		GL.MultMatrix (Camera.main.transform.worldToLocalMatrix);
		GL.Color (color);

		Path path = CirclePath (size, pos);
		float step = 0.02f;
		Vector3[] quad = null;
		for (float i = 0; i < 1f; i += step) {
			Vector2 p1 = path.GetPointAtPercent (i);
			Vector2 p2 = path.GetPointAtPercent (i + step);
			if (quad == null) {
				quad = MakeQuad (Camera.main.transform, p1, p2, thickness);
				foreach (Vector3 v in quad) {
					GL.Vertex (v);
				}
			} else {
				Vector3[] newquad = MakeQuad (Camera.main.transform, p1, p2, thickness);
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

	public Path CirclePath(float size, Vector2 position){
		Path path = new Path (new Vector2 (0f, -size));
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

	public Vector3[] MakeQuad(Transform transform, Vector3 s, Vector3 e, float w) {
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

public class ColorSwitchTrailEffect : TrailEffect {
	private TrailRenderer trail;
	private PlayerScript playerScript;

	public override void Create (GameObject player){
		GameObject g = Resources.Load<GameObject> ("TrailEffects/ColorSwitchTrailEffect");
		playerScript = player.GetComponent<PlayerScript> ();

		GameObject white = GameObject.Instantiate (g) as GameObject;
		white.transform.parent = player.transform;
		white.transform.position = player.transform.position;
		trail = white.GetComponent<TrailRenderer> ();
		trail.material.color = Color.white;
		trail.sortingOrder = -1;

		PlayerScript.afterColorChange += SwitchColorTrail;
	}

	void SwitchColorTrail(){
		trail.material.color = playerScript.GetColor ();
	}
}


