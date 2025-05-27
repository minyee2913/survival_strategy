using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class RollIndicator : MonoBehaviour
{
    [SerializeField]
    Image displayRate;

    [SerializeField]
    PlayerMovement movement;

    void Update()
    {
        MatchDisplay();
    }

    void MatchDisplay()
    {
        displayRate.fillAmount = (movement.rollCool.time - movement.rollCool.timeLeft()) / movement.rollCool.time;
    }

}
