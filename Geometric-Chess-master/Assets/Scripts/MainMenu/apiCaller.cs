using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class apiCaller : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            getUserDet();
        }
    }

    public void createUser() => StartCoroutine(createNewUser_Coroutine());

    public void getUserDet() => StartCoroutine(getUserDeatails_coroutine());


    IEnumerator createNewUser_Coroutine()
    {
        Debug.Log("creating user");

        string url = "https://chessgame-backend.herokuapp.com/api/createUser";
        WWWForm form = new WWWForm();
        form.AddField("Phone", playerPermData.getLocalId());

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

    IEnumerator getUserDeatails_coroutine()
    {
        Debug.Log("getting details");
        
      

        string uri = "https://chessgame-backend.herokuapp.com/api/getUserDetails/" + playerPermData.getLocalId();

        Debug.Log(uri);
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
              
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                //[{ "UserId":"dileep1643803180635",
                //        "UserName":"Dileep Kumar",
                //        "Email":"dileep@gmail.com",
                //        "DisplayImg":"test",
                //        "Joining":0,
                //        "LastLogin":0,
                //        "Phone":null,
                //        "spinWin":null,
                //        "password":"$2a$10$1r6LVwI8UevMjuRwU5HnVO6fDUUoV6KQfjq011JmansT/tl9tymMK","" +
                //        "otp":null,
                //        "tenentId":"1",
                //        "Points":100,
                //        "Won":0,
                //        "Lose":0,
                //        "Drawn":0,
                //        "Total":0,
                //        "LastGame":0,
                //        "MatchPoints":100,
                //        "Coins":null,
                //        "LastSpinTime":null}]

               
                JSONNode node = JSON.Parse(request.downloadHandler.text);

                //Debug.Log(node[0]["Email"].ToString().Substring(1, (node[0]["Email"].ToString().Length - 2)));

                playerPermData.setEmail(node[0]["Email"].ToString().Substring(1, (node[0]["Email"].ToString().Length - 2)));
                playerPermData.setPhoneNumber(node[0]["Phone"].ToString());
                playerPermData.setUserName(node[0]["UserName"].ToString());

              
                playerPermData.setMoney(int.Parse( node[0]["Coins"].ToString()));
                playerPermData.setScore(node[0]["Points"].ToString());

               

            }
        }
    }
}
