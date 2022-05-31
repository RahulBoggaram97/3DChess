using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class endGameApiCAller : MonoBehaviour
{
    public string MatchWonConstString = "won";
    public  string MatchLossConstString = "lost";

    


    public void sendOnlineMatchStat(string matchstat)
    {
        StartCoroutine(sendOnlineEndMatchStatus_Coroutine(matchstat));
    }

    public void sendTournamentMatchStat(string matchstat)
    {
        StartCoroutine(sendTournamentEndMatchStatus_Coroutine(matchstat));
    }


    IEnumerator sendOnlineEndMatchStatus_Coroutine(string matchStatus)
    {
        Debug.Log("sending match data");

        string url = "https://chessgame-backend.herokuapp.com/api/addOnlineMatchStatus";


        WWWForm form = new WWWForm();
        form.AddField("Status", matchStatus);
        form.AddField("UserId", playerPermData.getLocalId());

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //debugText.text = request.error;
                Debug.Log(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                //debugText.text = request.error;
                //authManager.instance.updateLoginState(loginState.loggedIn);
                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    IEnumerator sendTournamentEndMatchStatus_Coroutine(string matchStatus)
    {
        Debug.Log("sending match data");

        string url = "https://chessgame-backend.herokuapp.com/api/addTournamentStatus";
        WWWForm form = new WWWForm();
        form.AddField("Status", matchStatus);
        form.AddField("UserId", playerPermData.getLocalId());
        form.AddField("T_Id", playerPermData.getCurrentTournamentId());

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //debugText.text = request.error;
                Debug.Log(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                //debugText.text = request.error;
                //authManager.instance.updateLoginState(loginState.loggedIn);
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
