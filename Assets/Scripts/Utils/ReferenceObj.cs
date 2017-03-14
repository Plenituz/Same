using UnityEngine;
using System.Collections;

public class ReferenceObj : MonoBehaviour {
	public string[] names;
	public object[] objects;

	public object GetReferenceByName(string name){
		for (int i = 0; i < names.Length; i++) {
			if (names [i].Equals (name)) {
				return objects [i];
			}
		}
		print ("Reference not found");
		return null;
	}
}
