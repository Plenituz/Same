using System;
using UnityEngine;
using System.Reflection;

[ExecuteInEditMode]
public abstract class GameObstacle : MonoBehaviour{
	private Type playModeType;

	public bool reset;
	public float startAtTime = 0f;

	public abstract Type GetPlayModeType ();
	public abstract void SetupEditMode ();
	public abstract void SetColor(Color color);
	public abstract Color GetColor ();

	void Update(){
		if (Application.isPlaying)
			return;
		
		if (reset)
			reset = false;
		SetupEditMode ();
	}

	void Start(){
		if (Application.isPlaying) {
			playModeType = GetPlayModeType ();
			//gameObject.AddComponent<typeof(playModeType)>();
			MethodInfo method = typeof(GameObject).GetMethod("AddComponent", Type.EmptyTypes);
			method = method.MakeGenericMethod(playModeType);
			object gameModeScript = method.Invoke (gameObject, null);
			foreach(FieldInfo field in this.GetType().GetFields()){
				try{
					gameModeScript.GetType ().GetField (field.Name).SetValue (gameModeScript, field.GetValue (this));
				}catch(Exception){
					print ("Dis a dorian qu'il y a un pb : public field not passed correctly");
				}
			}

			Destroy (this);
		}
		SetupEditMode ();
	}
}


