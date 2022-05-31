using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class tournamentSlideManager : MonoBehaviour
{
    public Animator slideBarAnimator;

    public void SlideTheBar()
    {
        slideBarAnimator.SetTrigger("slider");
    }

    public void ReverseSlideThebar()
    {
        slideBarAnimator.SetTrigger("slider reverse");
    }
}
