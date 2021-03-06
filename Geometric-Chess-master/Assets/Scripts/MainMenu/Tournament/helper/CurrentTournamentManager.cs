using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CurrentTournamentManager : MonoBehaviour
{
    public tourApiManager tournamenetApi;
    public Text tournamenetNameText;

    private void OnEnable()
    {
        tournamenetNameText.text = playerPermData.getCurrentTournamentId();
        tournamenetApi.getPlayersinTournament(playerPermData.getCurrentTournamentId());
    }
}
