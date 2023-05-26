using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectPage : MonoBehaviour
{

    public TMP_Text displayNameText;
    public Button selectButton;
    public GameObject requirementPanel;
    public TMP_Text requirementText;

    public LevelSetSCO levelSetSCO;

    LevelSelectController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<LevelSelectController>();

        //if not unlocked
        if (JsonHelper.GetAllRecord().totalStars < levelSetSCO.requiredStars)
        {
            if (!UserSettings.cheat)
            {
                selectButton.interactable = false;
            }

            requirementPanel.SetActive(true);
            requirementText.text = levelSetSCO.requiredStars.ToString();
        }
    }

    public void OnClick()
    {
        LevelSelectController controller = FindObjectOfType<LevelSelectController>();

        if (controller)
        {
            controller.BringUpLevelButtonScroller(levelSetSCO);
        }
    }

}
