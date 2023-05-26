using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TMP_Text bestTimeText;
    public TMP_Text bestTimeTextMs;
    public TMP_Text bestCrystalText;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public TMP_Text levelNameText;

    public GameObject disabledPanel;
    public Color yellow;

    public int levelIndex;

    public void GoToLevel()
    {
        TopManager.levelIndex = levelIndex;
        TopManager.StartSet();
    }
}
