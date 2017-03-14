using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour {
	public Text mainText;
	public Text textButton1;
	public Text textButton2;
	public Image[] borders;
	public GameObject but1Click;

	private int completion;
	private int world;
	private int lastLevel;

	void Start () {
		PlayerScript.Reset ();
		Time.timeScale = 1f;
		GameObject lastLevelData = GameObject.FindGameObjectWithTag ("AlwaysThere");
		ReferenceObj reference = lastLevelData.GetComponent<ReferenceObj> ();
		Data data = Data.GetInstance ();
		//Level_mX_XX
		string lastLevelName = reference.GetReferenceByName("LastLevelName").ToString();
		lastLevel = int.Parse(lastLevelName.Substring(9));
		completion = ((int)reference.GetReferenceByName ("Completion")) + 1;
		world = int.Parse (lastLevelName.ToCharArray () [7].ToString());

		if (data.completedLevels [world - 1, lastLevel - 1] < completion) {
			data.completedLevels [world - 1, lastLevel - 1] = completion;
			Data.SetInstance (data);
		}
		Data.CheckForWorldAndPowerUnlocking ();
		Destroy (lastLevelData);

		GameObject bg = new GameObject("Background");
		bg.transform.position = Vector3.zero;
		bg.transform.localScale = new Vector3 (P.pocSGame (1f, Side.W), P.pocSGame (1f, Side.H), 1f);
		SpriteRenderer srBg = bg.AddComponent<SpriteRenderer> ();
		srBg.sortingOrder = -100;
		srBg.sprite = Resources.Load<Sprite> ("pixel");
		if (completion - 1 == (int)Completion.DIED) {
			srBg.color = Color.black;
		} else {
			srBg.color = Color.white;
			mainText.text = "LEVEL COMPLETE";
			textButton1.text = "Next";
			mainText.color = Color.black;
			textButton1.color = Color.black;
			textButton2.color = Color.black;
			for (int i = 0; i < borders.Length; i++) {
				borders [i].color = Color.black;
			}
		}
		mainText.canvasRenderer.SetAlpha (0f);
		mainText.CrossFadeAlpha (1f, 0.8f, true);
		StartCoroutine ("go");
		if ((completion - 1 != (int)Completion.DIED) && lastLevel == 20 && world == WorldSelectBanner.maxWorld) {
			Destroy (but1Click);
			textButton1.text = "No next lvl";
		}
	}

	IEnumerator go(){
		for (int i = 0; i < borders.Length; i++) {
			borders [i].canvasRenderer.SetAlpha (0f);
		}
		textButton1.canvasRenderer.SetAlpha (0f);
		textButton2.canvasRenderer.SetAlpha (0f);
		yield return new WaitForSeconds (1f);
		for (int i = 4; i < borders.Length; i++) {
			borders [i].CrossFadeAlpha (1f, 0.8f, true);
		}
		textButton1.CrossFadeAlpha (1f, 0.8f, true);
		yield return new WaitForSeconds (0.1f);
		for (int i = 0; i < 4; i++) {
			borders [i].CrossFadeAlpha (1f, 0.8f, true);
		}
		textButton2.CrossFadeAlpha (1f, 0.8f, true);
	}

	public void TryAgain(){
		print ("try");
		if (completion - 1 == (int)Completion.DIED) {
			//try again
			print("try again");
			SceneManager.LoadScene("Level_m" + world + "_" + lastLevel);
		} else {
			//next
			print("next");
			SceneManager.LoadScene("Level_m" + (lastLevel == 20 ? world+1 : world) + "_" + (lastLevel == 20 ? 1 : lastLevel+1));
		}
	}

	public void MainMenu(){
		SceneManager.LoadScene ("MainMenu");
	}
}
