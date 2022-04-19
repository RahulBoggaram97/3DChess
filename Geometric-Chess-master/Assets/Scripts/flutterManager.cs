using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class flutterManager : MonoBehaviour, IEventSystemHandler
{
    public string scene1;
    public string scene2;
    public string scene3;

    public void loadSinglePlayer()
    {
        SceneManager.LoadScene(scene1);
    }

    public void loadMultiPlayer()
    {
        SceneManager.LoadScene("scene2");

    }

    public void loadPuzzle()
    {
        SceneManager.LoadScene("scene3");
    }
}
