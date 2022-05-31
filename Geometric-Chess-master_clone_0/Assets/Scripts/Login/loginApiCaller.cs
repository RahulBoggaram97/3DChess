using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;



public class loginApiCaller : MonoBehaviour
{

    private void Start()
    {
        Debug.Log(playerPermData.getLocalId());
        if(playerPermData.getLocalId() != string.Empty)
        {
            startTheMainGame();
        }
    }


    [Header("RegisterUser")]
    public InputField userNameField;
    public InputField EmailField;
    public InputField PasswordField;
    public InputField ConfirmPasswardField;

    [Header("Login")]
    public InputField EmailLoginField;
    public InputField passwordLoginField;


    [Header("debug")]
    public Text debugTextLogin;
    public Text debugTextRegister;

    public void registerUser()
    {
        if(userNameField.text != string.Empty)
        {
            if(EmailField.text != string.Empty)
            {
                if(PasswordField.text != string.Empty)
                {
                    if(ConfirmPasswardField.text == PasswordField.text)
                    {
                        StartCoroutine(registernewUser_Coroutine());
                    }
                }
            }
        }
    }

    public void loginUser()
    {
        if(EmailLoginField.text != string.Empty)
        {
            if(passwordLoginField.text != string.Empty)
            {
                StartCoroutine(loginUser_Coroutine());
            }
        }
    }


    IEnumerator registernewUser_Coroutine()
    {
        Debug.Log("registring user");

        string url = "https://chessgame-backend.herokuapp.com/register";
        WWWForm form = new WWWForm();
        form.AddField("UserName", userNameField.text);
        form.AddField("Email", EmailField.text);
        form.AddField("password", PasswordField.text);

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
                //{ "userid":"user1653557820441"}
                Debug.Log(request.downloadHandler.text);

                JSONNode node = JSON.Parse(request.downloadHandler.text);

                Debug.Log(node["userid"].ToString());
                playerPermData.setLocalId(node["userid"].ToString().Substring(1, (node["userid"].ToString().Length - 2)));

                Debug.Log(playerPermData.getLocalId() + " this is the loacl id");

                startTheMainGame();

            }
        }
    }

    IEnumerator loginUser_Coroutine()
    {
        Debug.Log("loging user in");

        string url = "https://chessgame-backend.herokuapp.com/login";
        WWWForm form = new WWWForm();
     
        form.AddField("Email", EmailLoginField.text);
        form.AddField("password", passwordLoginField.text);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //debugText.text = request.error;
                Debug.Log(request.error);
                Debug.Log(request.downloadHandler.text);
               
                if (request.downloadHandler.text == "incorrect password")
                {
                    debugTextLogin.text = "Password is incorrect!!";
                }
                if (request.downloadHandler.text == "user not present")
                {
                    debugTextLogin.text = "User does not exist, Register.";
                }
            }
            else
            {
                //Success Login
                Debug.Log(request.downloadHandler.text);

                JSONNode node = JSON.Parse(request.downloadHandler.text);

                
                if (request.downloadHandler.text == "incorrect password")
                {
                    debugTextLogin.text = "Password is incorrect!!";
                }
                else if (request.downloadHandler.text == "user not present")
                {
                    debugTextLogin.text = "User does not exist, Register.";
                }
                else
                {
                    //{ "user_id":"user321653889416298"}
                    Debug.Log(node["user_id"].ToString().Substring(1, (node["user_id"].ToString().Length - 2)));
                    playerPermData.setLocalId(node["user_id"].ToString().Substring(1, (node["user_id"].ToString().Length - 2)));
                     startTheMainGame();
                }

            }
        }
    }

   void startTheMainGame()
    {
        
        SceneManager.LoadScene("Main Menu");
    }
}
