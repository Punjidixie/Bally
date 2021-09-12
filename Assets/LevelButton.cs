using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public Text bestTimeText;
    public Text bestTimeTextMs;
    public Text bestCrystalText;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public Text levelNameText;

    public GameObject disabledPanel;
    public Color yellow;

    public string levelName;

    public void GoToLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
