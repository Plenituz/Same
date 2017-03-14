using UnityEngine;
using System.Collections;

public class Reference : MonoBehaviour {
	public string[] names;
	public GameObject[] objects;

	public GameObject GetReferenceByName(string name){
		for (int i = 0; i < names.Length; i++) {
			if (names [i].Equals (name)) {
				return objects [i];
			}
		}
		print ("Reference not found");
		return null;
	}
}
