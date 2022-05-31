using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class profileManager : MonoBehaviour
{
    [Header("otherScripts")]
    public apiCaller _apiCaller;
    public uploadprofilPicture profileUploader;

    [Header("Text feilds")]
    public Text userId;  
    public Text Mail;
    public Text Phone;
    public Text CoinBalance;
    public Text Score;

    [Header("Edit Elements")]
    public InputField playerNameField;
    public Image profilePic;

    public Button changeProfileButton;



    private void OnEnable()
    {
        _apiCaller.getUserDet();

        userId.text = playerPermData.getLocalId();
        getPlayerName();
        StartCoroutine(DownloadImage(playerPermData.getProfilePicUrl()));
        Mail.text = playerPermData.getEmail();
        Phone.text = playerPermData.getPhoneNumber();
        CoinBalance.text = playerPermData.getMoney().ToString();
        Score.text = playerPermData.getScore();
    }

    public void getPlayerName()
    {
        string defaultName = string.Empty;
        if (playerNameField != null)
        {
            if (PlayerPrefs.HasKey(playerPermData.USERNAME_PREF_KEY))
            {
                defaultName = playerPermData.getUserName();
                playerNameField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;


    }

    public void setPlayerName()
    {
        if (playerNameField.text != string.Empty)
        {
            PhotonNetwork.NickName = playerNameField.text;
            playerPermData.setUserName(playerNameField.text);
        }
        else
        {

            return;
        }
    }

    public IEnumerator DownloadImage(string downloadUrl)
    {
        
        Debug.Log("Downloading image");

        Debug.Log(downloadUrl);

        Texture2D downloadedTexture = null;
        

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(downloadUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("cant connect oof");
            Debug.Log(request.error);
        }
        else
        {
            downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log("image downladed");
           
        }

        if (downloadedTexture != null)
            profilePic.sprite = Sprite.Create(downloadedTexture, new Rect(0f, 0f, downloadedTexture.width, downloadedTexture.height), new Vector2(0.5f, 0.5f), 100f);
        else Debug.Log("texuture is null");


    }

    public void uploadPhoto()
    {
        profileUploader.imagePicker();
    }

    bool isEditing = false;

    public void edit()
    {
        if (isEditing) 
        {
            playerNameField.interactable = false;
            changeProfileButton.interactable = false;
            isEditing = !isEditing;
        }
        else
        {
            playerNameField.interactable = true;
            changeProfileButton.interactable = true;
            isEditing = !isEditing;
        }
        
    }

    public void logout()
    {
        playerPermData.setLocalId(string.Empty);
        Debug.Log(playerPermData.getLocalId());
        SceneManager.LoadScene("Login");
    }
}
