using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonsScroller : MonoBehaviour
{
    public GameObject content;
    public GameObject buttonsContainer;
    public GameObject levelButtonPrefab;

    public void PopulateWith(LevelSetSCO levelSetSCO)
    {

        // EXPLANATIONS: Content is the actual thing that will be scrolled.
        // Therefore the scrollable height is numer of levels x height of each box (100).
        // Plus two 12s for the margins at the top and bottom.

        int nLevels = levelSetSCO.levels.Length;
        float contentHeight = nLevels * 100 + 24;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentHeight);

        // Reset the container
        int childs = buttonsContainer.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(buttonsContainer.transform.GetChild(i).gameObject);
        }
        TopManager.levelSetSCO = levelSetSCO;

        // Populate the container
        for (int i = 0; i < nLevels; i++)
        {

            int levelNumber = i + 1;
            GameObject levelButtonObject = Instantiate(levelButtonPrefab);
            LevelButton levelButton = levelButtonObject.GetComponent<LevelButton>();
            levelButtonObject.transform.SetParent(buttonsContainer.transform);
            levelButton.levelNameText.text = "Level " + levelNumber;
            levelButton.levelIndex = i;

            LevelSCO levelSCO = levelSetSCO.levels[i];
            string name = levelSCO.sceneName;

            LevelRecord tempLevelRecord = JsonHelper.GetLevelRecord(name);

            //Unlocked it right here if it's an ordered Lv1
            if (levelSetSCO.consececutiveUnlock && i == 0)
            {
                tempLevelRecord.unlocked = true;
            }

            //If unlocked
            if (tempLevelRecord.unlocked)
            {

                levelButton.disabledPanel.SetActive(false);
                levelButton.GetComponent<Button>().interactable = true;

                //If completed
                if (tempLevelRecord.completed)
                {
                    //calculate time taken
                    float timePassed = tempLevelRecord.time;
                    float minutes = Mathf.Floor(timePassed / 60f);
                    float seconds = Mathf.Floor(timePassed - 60 * minutes);
                    float millisecs = Mathf.Floor((timePassed - 60 * minutes - seconds) * 100);

                    string minutesString = minutes.ToString();
                    string secondsString;
                    string millisecsString;

                    if (seconds < 10){ secondsString = "0" + seconds.ToString(); }
                    else { secondsString = seconds.ToString(); }
     
                    if (millisecs < 10) { millisecsString = "0" + millisecs.ToString(); }
                    else { millisecsString = millisecs.ToString(); }
  
                    levelButton.bestTimeText.text = minutesString + ":" + secondsString;
                    levelButton.bestTimeTextMs.text = ":" + millisecsString;

                    //dealing with stars
                    if (tempLevelRecord.stars >= 1)
                    {
                        levelButton.star1.GetComponent<Image>().color = levelButton.yellow;
                    }
                    if (tempLevelRecord.stars >= 2)
                    {
                        levelButton.star2.GetComponent<Image>().color = levelButton.yellow;
                    }
                    if (tempLevelRecord.stars >= 3)
                    {
                        levelButton.star3.GetComponent<Image>().color = levelButton.yellow;
                    }

                    //dealing with crystals
                    levelButton.bestCrystalText.text = "x" + tempLevelRecord.crystals;
                }

            }

            //If not unlocked
            else
            {
                levelButton.disabledPanel.SetActive(true);
                levelButton.GetComponent<Button>().interactable = false;
            }

            if (UserSettings.cheat)
            {
                levelButton.GetComponent<Button>().interactable = true;
            }
        }
    }
}
