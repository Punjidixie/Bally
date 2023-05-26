using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndButtons : MonoBehaviour
{
    public GameObject nextButton;

    void Start()
    {
        if (TopManager.instantiated)
        {
            if (TopManager.levelIndex + 1 >= TopManager.levelSetSCO.levels.Length)
            {
                nextButton.SetActive(false);
            }
        }
    }
}
