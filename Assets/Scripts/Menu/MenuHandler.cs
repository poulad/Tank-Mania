using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

	public void Play()
    {
        SceneManager.LoadScene("Playground", LoadSceneMode.Single);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
