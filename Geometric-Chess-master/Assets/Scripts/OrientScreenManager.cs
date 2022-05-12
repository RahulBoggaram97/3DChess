using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrientScreenManager : MonoBehaviour
{
    public bool OrientScreen = true;

    public void Awake()
    {
      if(OrientScreen)  
        Screen.orientation = ScreenOrientation.LandscapeLeft;
      else 
            Screen.orientation = ScreenOrientation.Portrait;
    }

    public void BackButtonLoadScene(string loadThisScene)
    {
        SceneManager.LoadScene(loadThisScene);
    }
}
