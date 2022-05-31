using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrientScreenManager : MonoBehaviour
{
    public bool OrientScreen = true;

    public Text coinText;

    private void Start()
    {
        if(coinText != null)
        {
            coinText.text = playerPermData.getMoney().ToString();
        }
    }

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
