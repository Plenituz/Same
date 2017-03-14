using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	private LineRenderer lr;
	public Vector2 velocity;

	void Start () {
		lr = GetComponent<LineRenderer> ();
		GetComponent<Rigidbody2D> ().velocity = velocity;
	}
	
	void Update () {
		lr.SetPositions (new Vector3[] {
			ComplexNumber.rotate(transform.position, transform.position + new Vector3 (0.68f, 0f, 0f), Mathf.Deg2Rad*transform.rotation.eulerAngles.z),
			transform.position
		});
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Obstacle")) {
			Destroy (other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Boundary")) {
			Destroy (gameObject);
		}
	}
}
