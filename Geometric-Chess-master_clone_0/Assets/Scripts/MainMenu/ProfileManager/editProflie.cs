using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace com.impactionalGames.LudoInu
{
    public class editProflie : MonoBehaviour
    {
        public InputField nameField;
        public Image profilePic;

        public InputField otherNameField;
        public Image otherProfilePic;

        private void Start()
        {
            getPlayerName();
        }

        public void getPlayerName()
        {
            string defaultName = string.Empty;
            if (nameField != null)
            {
                if (PlayerPrefs.HasKey(playerPermData.USERNAME_PREF_KEY))
                {
                    defaultName = PlayerPrefs.GetString(playerPermData.USERNAME_PREF_KEY);
                    nameField.text = defaultName;
                }
            }

        }



        public void setPlayerName()
        {
            if (nameField.text != string.Empty)
            {
                PhotonNetwork.NickName = nameField.text;
                playerPermData.setUserName(nameField.text);
                otherNameField.text = nameField.text;

                otherProfilePic.sprite = profilePic.sprite;
            }
            else
            {

                return;
            }
        }
    }
}
