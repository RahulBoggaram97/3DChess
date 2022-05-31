using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Image musicimg;
    public Image musicimgNeg;

    public Image Sound;
    public Image SoundNeg;

    public void ToggleMusic()
    {
        if (musicimg.IsActive())
        {
            musicimgNeg.gameObject.SetActive(true);
            musicimg.gameObject.SetActive(false);
        }
        else
        {
            musicimg.gameObject.SetActive(true);
            musicimgNeg.gameObject.SetActive(false);
        }
    }

    public void ToggleSound()
    {
        if (Sound.IsActive())
        {
            SoundNeg.gameObject.SetActive(true);
            Sound.gameObject.SetActive(false);
        }
        else
        {
            Sound.gameObject.SetActive(true);
            SoundNeg.gameObject.SetActive(false);
        }
    }
}
