using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class lichessauthenticater : MonoBehaviour
{
    private void Start()
    {
        oAuth();
    }

    public void oAuth() => StartCoroutine(oauth_Coroutine());

    IEnumerator oauth_Coroutine()
    {
        Debug.Log("geting authiarised");



        string uri = "https://lichess.org/oauth";

        Debug.Log(uri);
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
