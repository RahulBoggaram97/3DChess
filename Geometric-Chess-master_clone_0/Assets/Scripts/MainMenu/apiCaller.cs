using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class apiCaller : MonoBehaviour
{
    
   

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

                //[{ "UserId":"user321653889416298",
                //        "Email":"user32@gmail.com",
                //        "Joining":"2022-05-30T05:43:36.000Z",
                //        "LastLogin":"",
                //        "UserName":"user",
                //        "DisplayImg":"https://i.stack.imgur.com/l60Hf.png",
                //        "password":"$2a$10$gyao6q6rtWDQbAEBiaDZ9ueTTlODsubbbCGWGiv4ea7.dPnCFBMnC",
                //        "Points":100,
                //        "Won":0,
                //        "Lose":0,
                //        "Total":0,
                //        "Drawn":0,
                //        "LastGame":"2022-05-30T05:43:36.000Z",
                //        "Coins":0,
                //        "LastSpinTime":""}]

               
                JSONNode node = JSON.Parse(request.downloadHandler.text);

                //Debug.Log(node[0]["Email"].ToString().Substring(1, (node[0]["Email"].ToString().Length - 2)));

                playerPermData.setEmail(node[0]["Email"].ToString().Substring(1, (node[0]["Email"].ToString().Length - 2)));
                playerPermData.setProfilePicUrl(node[0]["DisplayImg"].ToString().Substring(1, (node[0]["DisplayImg"].ToString().Length - 2)));
                playerPermData.setUserName(node[0]["UserName"].ToString().Substring(1, (node[0]["UserName"].ToString().Length - 2)));

              
                playerPermData.setMoney(int.Parse( node[0]["Coins"].ToString()));
                playerPermData.setScore(node[0]["Points"].ToString());

               

            }
        }
    }
}
