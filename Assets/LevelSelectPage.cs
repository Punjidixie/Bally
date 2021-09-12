using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPage : MonoBehaviour
{
    
    public string setName;

    public Text displayNameText;
    public Button selectButton;
    public GameObject requirementPanel;
    public Text requirementText;
    // Start is called before the first frame update
    void Start()
    {
        //if not unlocked
        if (PlayerPrefs.GetInt("TotalStar", 0) < DefaultSettings.GetRequiredStars(setName))
        {
            if (!UserSettings.cheat)
            {
                selectButton.interactable = false;
            }
            displayNameText.text = "-Locked-";
            requirementPanel.SetActive(true);
            requirementText.text = DefaultSettings.GetRequiredStars(setName).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
