using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script essentialy just dictates how the skin select UI works, and sets the value for PlayerPrefs' "Skin" key.
public class SkinSelectController : MonoBehaviour
{
    public GameObject scrollSnap;
    public GameObject content;
    SimpleScrollSnap simpleScrollSnap;
    ScrollRect simpleScrollSnapScrollRect;


    public Vector3 startPos;

    string[] skinNames;

    // Start is called before the first frame update
    void Start()
    {
        UserSettings.CheckDefaults();
        simpleScrollSnap = scrollSnap.GetComponent<SimpleScrollSnap>();
        simpleScrollSnapScrollRect = scrollSnap.GetComponent<ScrollRect>();

        startPos = Camera.main.transform.position;

        skinNames = new string[] { "WoodFace", "Wood", "Neptune", "Earth", "Kel", "Obama" };


        // Initialize
        for (int j = 0; j < skinNames.Length; j++)
        {
            SkinSelectBox sBox = content.transform.GetChild(j).GetComponent<SkinSelectBox>();
            if (skinNames[j] == PlayerPrefs.GetString("Skin"))
            {
                sBox.checkBox.SetActive(true);
                simpleScrollSnap.startingPanel = j;
            }
            else
            {
                sBox.checkBox.SetActive(false);
            }
            
        }


    }


    private void LateUpdate()
    {
        float trueCurrentPage = simpleScrollSnapScrollRect.horizontalNormalizedPosition * (simpleScrollSnap.NumberOfPanels - 1);

        float toXPosition = trueCurrentPage * 3f;

        Camera.main.transform.position = startPos + Vector3.right * toXPosition;

    }

    void SelectSkin(int i)
    {

        for (int j = 0; j < skinNames.Length; j++)
        {
            SkinSelectBox sBox = content.transform.GetChild(j).GetComponent<SkinSelectBox>();
            if (j == i)
            {
                sBox.checkBox.SetActive(true);
                PlayerPrefs.SetString("Skin", skinNames[j]);
            }
            else
            {
                sBox.checkBox.SetActive(false);
            }

        }

    }

    public void SelectCurrent()
    {
        SelectSkin(simpleScrollSnap.CurrentPanel);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Level Select");
    }
}
