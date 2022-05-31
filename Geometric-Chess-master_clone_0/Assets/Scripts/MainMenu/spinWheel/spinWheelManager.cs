using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using EasyUI.PickerWheelUI;

public enum spinState
{
    spinAble,
    timeCountDown
}


public class spinWheelManager : MonoBehaviour
{
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private Text uiSpinText;
    [SerializeField] private PickerWheel pickWheel;


    public spinState state;


    [Header("Panels")]
    public GameObject pickerWheelPanel;
    public GameObject timeToSpinWheelPanel;

    [Header("Text")]
    public Text nextTimeToSpinText;
    public Text wonCoinText;


    [Header("Other Scripts")]
    public getLastSpinApi spinApis;

    private void Start()
    {
        spinApis.getLastSpin();
        uiSpinButton.onClick.AddListener(spinWhell);

    }




    public void updateWheelState(spinState newState)
    {
        state = newState;

        switch (state)
        {
            case spinState.spinAble:
                handleSpinableState();
                break;
            case spinState.timeCountDown:
                handleTimeCountDownState();
                break;
        }


    }

    private void handleSpinableState()
    {
        pickerWheelPanel.SetActive(true);
        timeToSpinWheelPanel.SetActive(false);
    }

    private void handleTimeCountDownState()
    {
        pickerWheelPanel.SetActive(false);
        timeToSpinWheelPanel.SetActive(true);



        nextTimeToSpinText.text = timeLeftForNextSpin();


    }


    string timeLeftForNextSpin()
    {
        string currentTime = DateTime.Now.ToString("h:mm tt");
        Debug.Log(currentTime);
        int currenthours = int.Parse(currentTime.Substring(0, 1));
        int currentMinutes = int.Parse(currentTime.Substring(2, 2));
        string currentTt = currentTime.Substring(5, 2);


        if (currentTt == "PM")
            currenthours = currenthours + 12;


        int hourLeft = 23 - currenthours;
        int minutesLeft = 60 - currentMinutes;

        string timeLeft = hourLeft.ToString() + ":" + minutesLeft.ToString();

        return timeLeft;

    }


    public void spinWhell()
    {
        uiSpinButton.interactable = false;
        uiSpinText.text = "Spinning..";

        pickWheel.OnSpinEnd(wheelPiece =>
        {
            Debug.Log("Spin End: Type : " + wheelPiece.Label + ", Amount: " + wheelPiece.Amount);
            uiSpinButton.interactable = true;
            uiSpinText.text = "Spin";

            wonCoinText.text = wheelPiece.Amount.ToString();
            updateWheelState(spinState.timeCountDown);

            spinApis.addCoinsFromWheel(wheelPiece.Amount.ToString());
            

        });


        pickWheel.Spin();

    }



    public void ShowWheelToPlayer()
    {
        if (spinApis.canSpin)
        {
            Debug.Log("the can spin bool is: " + spinApis.canSpin);
            updateWheelState(spinState.spinAble);
        }
        else
        {
            Debug.Log("the can spin bool is: " + spinApis.canSpin);
            updateWheelState(spinState.timeCountDown);
        }
    }
}
