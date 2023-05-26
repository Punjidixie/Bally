using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public TMP_Text winningMessage;
    public TMP_Text timeTakenText;
    public TMP_Text timeTakenTextMs;
    public TMP_Text crystalsWinText;
    public GameObject winStar1;
    public GameObject winStar2;
    public GameObject winStar3;
    public Text bestTimeMessage;
    public Text bestCrystalMessage;

    LevelController levelController;

    void Awake()
    {
        levelController = FindObjectOfType<LevelController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetWinInfo(LevelRecord previousRecord)
    {

        StartCoroutine(WinCrystalStarAnimation());

        winningMessage.text = "Level Completed!\n" + levelController.setName + " " + levelController.levelNumber.ToString();

        //calculate time taken
        float minutes = Mathf.Floor(levelController.timePassed / 60f);
        float seconds = Mathf.Floor(levelController.timePassed - 60 * minutes);
        float millisecs = Mathf.Floor((levelController.timePassed - 60 * minutes - seconds) * 100);

        string minutesString = minutes.ToString();
        string secondsString;
        string millisecsString;
        if (seconds < 10)
        {
            secondsString = "0" + seconds.ToString();
        }
        else
        {
            secondsString = seconds.ToString();
        }
        if (millisecs < 10)
        {
            millisecsString = "0" + millisecs.ToString();
        }
        else
        {
            millisecsString = millisecs.ToString();
        }
        timeTakenText.text = minutesString + ":" + secondsString;
        timeTakenTextMs.text = ":" + millisecsString;

        //new record
        if (levelController.crystals > previousRecord.crystals)
        {
            bestCrystalMessage.gameObject.SetActive(true);
        }

        bool hasBestTime = levelController.timePassed < previousRecord.time;
        bool hasBest3sTime = levelController.timePassed < previousRecord.threeStarTime && levelController.crystalTracker.stars == 3;

        if (hasBestTime && hasBest3sTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best time & 3-star time!";
        }
        else if (hasBestTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best time!";
        }
        else if (hasBest3sTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best 3-star time!";
        }
    }

    public IEnumerator WinCrystalStarAnimation()
    {
        float t = 0;
        float spamRate = 30f; //crystals per second
        int currentStar = 0;
        while (t <= 1)
        {
            t += Time.unscaledDeltaTime / (levelController.crystals / spamRate);
            float number = Mathf.Round(Mathf.Lerp(0, levelController.crystals, t));
            crystalsWinText.text = number.ToString();

            if (number >= levelController.reqCrystal1 && currentStar < 1)
            {
                winStar1.GetComponent<Image>().color = levelController.crystalTracker.yellow;
                winStar1.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 1;
            }
            if (number >= levelController.reqCrystal2 && currentStar < 2)
            {
                winStar2.GetComponent<Image>().color = levelController.crystalTracker.yellow;
                winStar2.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 2;
            }
            if (number >= levelController.reqCrystal3 && currentStar < 3)
            {
                winStar3.GetComponent<Image>().color = levelController.crystalTracker.yellow;
                winStar3.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 3;
            }
            yield return null;
        }
        crystalsWinText.text = levelController.crystals.ToString();
    }
}
