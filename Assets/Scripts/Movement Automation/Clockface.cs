using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clockface : MonoBehaviour
{
    public GameObject hourHand;
    public GameObject minuteHand;

    float currentHour;
    float currentMinute;
    LevelController levelController;

    void DisplayTime(float hour, float minute)
    {
        if (hour > 12) { hour -= 12; }
        float trueHour = hour + minute / 60f;
        hourHand.transform.localEulerAngles = -360f * (trueHour / 12f) * Vector3.forward;
        minuteHand.transform.localEulerAngles = -360f * (minute / 60f) * Vector3.forward;

    }

    private void Start()
    {
        currentHour = 10;
        currentMinute = 0;
        levelController = FindObjectOfType<LevelController>();
    }

    private void Update()
    {
        float timeLeft = levelController.maxTime - levelController.timePassed;
        currentHour = Mathf.Floor(timeLeft / 60f);
        currentMinute = timeLeft - currentHour * 60f;
        DisplayTime(currentHour, currentMinute);
    }
}
