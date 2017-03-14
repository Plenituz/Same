using UnityEngine;
using System.Collections;

public class SetSizeAndPosPoc : MonoBehaviour {
	public Vector2 pos;
	public Vector2 size;

	void Start () {
		if (!P.isInit)
			P.init ();
		transform.position = new Vector2 (P.pocP (pos.x, Side.W), P.pocP (pos.y, Side.H));
		transform.localScale = new Vector3 (P.pocSGame (size.x, Side.W), P.pocSGame (size.y, Side.H), 1f);
	}
}
