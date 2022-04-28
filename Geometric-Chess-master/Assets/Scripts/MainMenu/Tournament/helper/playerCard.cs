using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerCard : MonoBehaviour
{
    public Text playerNameText;
    public Text winsText;

    public string playerName;
    public string playerWins;


    public void setAllText()
    {
        playerNameText.text = playerName;
        winsText.text = playerWins;
    }
}
