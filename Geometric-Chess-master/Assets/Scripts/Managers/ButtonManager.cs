using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {


	public GameObject profilePanel;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
			profilePanel.SetActive(false);
        }
    }


    public void LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void Quit() {
		Application.Quit();
	}

	public void ReloadScene() {
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
