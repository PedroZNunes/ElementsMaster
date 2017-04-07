using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevelManager : MonoBehaviour {

	public void LoadLevel (string levelName){
		SceneManager.LoadScene (levelName);
	}
}
