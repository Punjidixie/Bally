using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public class LevelSelectController : MonoBehaviour
{
    public Vector3[] lightingArray;
    public float normalY;
    public float normalZ;

    //level button scroller
    public GameObject levelButtonScroller;
    public GameObject levelButtonContent;
    public GameObject levelButtonContainer;
    public GameObject levelButtonPrefab;

    public GameObject scrollSnap;
    public GameObject scrollSnapBlocker;
    public Text totalStarText;

    public GameObject sceneLight;

    //3D scrolling
    SimpleScrollSnap simpleScrollSnap;
    ScrollRect simpleScrollSnapScrollRect;
    public float islandDistance;
    public float clickLeftShift;
    public float clickDownShift;
    
    public GameObject backButton;
    public GameObject totalStar;
    public GameObject pagination;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        normalY = Camera.main.transform.position.y;
        normalZ = Camera.main.transform.position.z;

        totalStarText.text = PlayerPrefs.GetInt("TotalStar", 0).ToString();
        simpleScrollSnap = scrollSnap.GetComponent<SimpleScrollSnap>();
        simpleScrollSnapScrollRect = scrollSnap.GetComponent<ScrollRect>();
    }

    // Update island lightings
    void Update()
    {
        float trueCurrentPage = Mathf.Clamp(simpleScrollSnapScrollRect.horizontalNormalizedPosition * (simpleScrollSnap.NumberOfPanels - 1), 0, 2);

        Quaternion previousPageLight = Quaternion.Euler(lightingArray[Mathf.FloorToInt(trueCurrentPage)].x, lightingArray[Mathf.FloorToInt(trueCurrentPage)].y, lightingArray[Mathf.FloorToInt(trueCurrentPage)].z);
        Quaternion nextPageLight = Quaternion.Euler(lightingArray[Mathf.CeilToInt(trueCurrentPage)].x, lightingArray[Mathf.CeilToInt(trueCurrentPage)].y, lightingArray[Mathf.CeilToInt(trueCurrentPage)].z);
        sceneLight.transform.rotation = Quaternion.Slerp(previousPageLight, nextPageLight, trueCurrentPage - Mathf.Floor(trueCurrentPage));

    }

    // Update island locations
    private void LateUpdate()
    {
        float trueCurrentPage = simpleScrollSnapScrollRect.horizontalNormalizedPosition * (simpleScrollSnap.NumberOfPanels - 1);

        float toXPosition = trueCurrentPage * islandDistance;
        float toYPosition = normalY - (simpleScrollSnap.gameObject.GetComponent<RectTransform>().anchoredPosition.x / 150f) * clickDownShift;
        toXPosition -= (simpleScrollSnap.gameObject.GetComponent<RectTransform>().anchoredPosition.x / 150f) * clickLeftShift;

        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, normalZ);


    }

    public void BringUpLevelButtonScroller(string setName)
    {
        int n = DefaultSettings.GetNumberOfLevels(setName);

        scrollSnap.GetComponent<Animator>().SetTrigger("MakeWay");
        scrollSnap.GetComponent<Animator>().ResetTrigger("ComeBack");
        scrollSnapBlocker.SetActive(true);

        levelButtonScroller.GetComponent<Animator>().SetTrigger("ComeIn");
        levelButtonScroller.GetComponent<Animator>().ResetTrigger("ComeOut");
        levelButtonScroller.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        backButton.SetActive(true);
        totalStar.SetActive(false);
        pagination.SetActive(false);

        float belowScreen = n * 100 + 24 > 0 ? n * 100 + 24 : 0;

        levelButtonContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, belowScreen);

        int childs = levelButtonContainer.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(levelButtonContainer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < n; i++)
        {
            int levelNumber = i + 1;
            GameObject levelButtonObject = Instantiate(levelButtonPrefab);
            LevelButton levelButton = levelButtonObject.GetComponent<LevelButton>();
            levelButtonObject.transform.SetParent(levelButtonContainer.transform);
            levelButton.levelNameText.text = "Level " + levelNumber;
            levelButton.levelName = setName + levelNumber;
            //completed level
            if (PlayerPrefs.GetInt(setName + (levelNumber) + "Completed", 0) == 1)
            {
                levelButton.disabledPanel.SetActive(false);
                levelButton.GetComponent<Button>().interactable = true;

                //calculate time taken
                float timePassed = PlayerPrefs.GetFloat(setName + levelNumber + "BestTime", Mathf.Infinity);
                float minutes = Mathf.Floor(timePassed / 60f);
                float seconds = Mathf.Floor(timePassed - 60 * minutes);
                float millisecs = Mathf.Floor((timePassed - 60 * minutes - seconds) * 100);

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
                levelButton.bestTimeText.text = minutesString + ":" + secondsString;
                levelButton.bestTimeTextMs.text = ":" + millisecsString;

                if (PlayerPrefs.GetInt(setName + levelNumber + "BestStar", -1) >= 1)
                {
                    levelButton.star1.GetComponent<Image>().color = levelButton.yellow;
                }
                if (PlayerPrefs.GetInt(setName + levelNumber + "BestStar", -1) >= 2)
                {
                    levelButton.star2.GetComponent<Image>().color = levelButton.yellow;
                }
                if (PlayerPrefs.GetInt(setName + levelNumber + "BestStar", -1) >= 3)
                {
                    levelButton.star3.GetComponent<Image>().color = levelButton.yellow;
                }

                levelButton.bestCrystalText.text = "x" + PlayerPrefs.GetInt(setName + levelNumber + "BestCrystal", -1);
            }
            //not completed level, last level not completed as well (not unlocked)
            else if (levelNumber != 1 && PlayerPrefs.GetInt(setName + (levelNumber - 1) + "Completed", 0) == 0)
            {
                levelButton.disabledPanel.SetActive(true);
                if (!UserSettings.cheat)
                {
                    levelButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    levelButton.GetComponent<Button>().interactable = true;
                }
                levelButton.star1.GetComponent<Image>().color = Color.white;
                levelButton.star2.GetComponent<Image>().color = Color.white;
                levelButton.star3.GetComponent<Image>().color = Color.white;
                levelButton.bestTimeText.text = "??:??";
                levelButton.bestTimeTextMs.text = ":??";
                levelButton.bestCrystalText.text = "x?";

            }
            //not completed level but last level completed (unlocked)
            else if (levelNumber != 1 && PlayerPrefs.GetInt(setName + (levelNumber) + "Completed", 0) == 0 && PlayerPrefs.GetInt(setName + (levelNumber - 1) + "Completed", 0) == 1)
            {
                levelButton.disabledPanel.SetActive(false);
                levelButton.GetComponent<Button>().interactable = true;

                levelButton.star1.GetComponent<Image>().color = Color.white;
                levelButton.star2.GetComponent<Image>().color = Color.white;
                levelButton.star3.GetComponent<Image>().color = Color.white;
                levelButton.bestTimeText.text = "??:??";
                levelButton.bestTimeTextMs.text = ":??";
                levelButton.bestCrystalText.text = "x?";
            }
            //level 1 is always unlocked
            else if (levelNumber == 1)
            {
                levelButton.disabledPanel.SetActive(false);
                levelButton.GetComponent<Button>().interactable = true;

                levelButton.star1.GetComponent<Image>().color = Color.white;
                levelButton.star2.GetComponent<Image>().color = Color.white;
                levelButton.star3.GetComponent<Image>().color = Color.white;
                levelButton.bestTimeText.text = "??:??";
                levelButton.bestTimeTextMs.text = ":??";
                levelButton.bestCrystalText.text = "x?";
            }
        }
    }
    
    public void BringBackLevelButtonScroller()
    {
        levelButtonScroller.GetComponent<Animator>().SetTrigger("ComeOut");
        levelButtonScroller.GetComponent<Animator>().ResetTrigger("ComeIn");

        scrollSnap.GetComponent<Animator>().SetTrigger("ComeBack");
        scrollSnap.GetComponent<Animator>().ResetTrigger("MakeWay");
        scrollSnapBlocker.SetActive(false);

        backButton.SetActive(false);
        totalStar.SetActive(true);
        pagination.SetActive(true);
    }
}
