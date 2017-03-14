using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ParentCanvasPosition : MonoBehaviour {
	public Transform parent;
	private Vector3 basePos;
	private RectTransform rectTransform;

	void Start(){
		rectTransform = GetComponent<RectTransform> ();
		try{
			basePos = parent.position;
		}catch(UnassignedReferenceException){
			print ("Reference not set for " + name);
		}

	}

	void Update () {
		rectTransform.position = parent.position - basePos;
	}
}
