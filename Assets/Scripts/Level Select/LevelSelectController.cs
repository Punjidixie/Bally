using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.SceneManagement;
using TMPro;

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
    public TMP_Text totalStarText;

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
        Time.timeScale = 1;

        normalY = Camera.main.transform.position.y;
        normalZ = Camera.main.transform.position.z;

        totalStarText.text = JsonHelper.GetAllRecord().totalStars.ToString();
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

    //Called from clicking on the middle of a page (invisible huge button)
    public void BringUpLevelButtonScroller(LevelSetSCO levelSetSCO)
    {
        //Movement stuffs
        scrollSnap.GetComponent<Animator>().SetTrigger("MakeWay");
        scrollSnap.GetComponent<Animator>().ResetTrigger("ComeBack");
        scrollSnapBlocker.SetActive(true);

        levelButtonScroller.GetComponent<Animator>().SetTrigger("ComeIn");
        levelButtonScroller.GetComponent<Animator>().ResetTrigger("ComeOut");
        levelButtonScroller.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        backButton.SetActive(true);
        totalStar.SetActive(false);
        pagination.SetActive(false);

        //Populating the scroller
        levelButtonScroller.GetComponent<LevelButtonsScroller>().PopulateWith(levelSetSCO);
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

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
