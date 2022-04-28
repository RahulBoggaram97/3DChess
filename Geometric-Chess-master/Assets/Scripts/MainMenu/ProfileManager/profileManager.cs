using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class profileManager : MonoBehaviour
{
    [Header("otherScripts")]
    public apiCaller _apiCaller;

    [Header("Text feilds")]
    public Text userId;
    public Text Name;
    public Text Mail;
    public Text Phone;
    public Text CoinBalance;
    public Text Score;

    private void OnEnable()
    {
        _apiCaller.getUserDet();

        userId.text = playerPermData.getLocalId();
        Name.text = playerPermData.getUserName();
        Mail.text = playerPermData.getEmail();
        Phone.text = playerPermData.getPhoneNumber();
        CoinBalance.text = playerPermData.getMoney().ToString();
        Score.text = playerPermData.getScore();
    }
}
