using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class tournamentCard : MonoBehaviour
{
    [Header("TextElemnets")]
    public Text t_idText;
    public Text t_nameText;
    public Text descriptionText;
    public Text t_feeText;



    [Header("Strings")]
    public string t_id;
    public string t_name;
    public string totalPoints;
    public string totalGames;
    public string description;
    public string t_img;
    public string status;
    public int t_fee;
    public int maxplayers;
    public string gameTime;


    public void setAllTheText()
    {
        
       setSingleText(t_id, ref t_idText);
        setSingleText(t_name, ref t_nameText);
        setSingleText(description, ref descriptionText);
        setSingleText(t_fee.ToString(), ref t_feeText);

        
    }

    public void setSingleText(string theStringPart, ref Text textElement)
    {
        if(textElement != null)
        {
            textElement.text = theStringPart;
        }
    }

    public void joinTournament()
    {
        GameObject.Find("Manager").GetComponent<tourUiHangler>().joinTournamentOnPhoton(t_id);
        tourUiHangler.setCurrentTournamentId(t_id);
    }


}
