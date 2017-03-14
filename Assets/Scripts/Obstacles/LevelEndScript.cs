using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelEndScript : MonoBehaviour {
	public float time;

	void Start () {
		StartCoroutine ("End");
	}

	IEnumerator End(){
		yield return new WaitForSeconds (time);
		GameStartup startup = Camera.main.GetComponent<GameStartup> ();
		CreateEndLevelObj (SceneManager.GetActiveScene ().name, startup.easyMode ? Completion.FINISHED_EASY : Completion.FINISHED);
		SpawnEndAnim (Color.white);
		yield return new WaitForSeconds (0.7f);
		SceneManager.LoadScene ("DeathScreen");
	}

	public static void CreateEndLevelObj(string lastLevelName, Completion completion){
		GameObject g = new GameObject ("LastLevelData");
		ReferenceObj r = g.AddComponent<ReferenceObj> ();
		r.names = new string[]{ "LastLevelName", "Completion" };
		r.objects = new object[]{ lastLevelName, completion };
		g.tag = "AlwaysThere";
		DontDestroyOnLoad (g);
	}

	public static void SpawnEndAnim(Color color){
		GameObject square = Instantiate (Resources.Load<GameObject> ("SquareEnd")) as GameObject;
		square.GetComponent<SquareEnd> ().color = color;
		GameObject ui = GameObject.FindGameObjectWithTag ("PowerUI");
		for (int i = 0; i < ui.transform.childCount; i++) {
			if (ui.transform.GetChild (i).name.Contains ("Power")) {
				ui.transform.GetChild (i).GetComponent<PowerScript> ().ScaleOut ();
			}
		}
	}
}

public enum Completion{
	DIED,
	FINISHED_EASY,
	FINISHED
}
