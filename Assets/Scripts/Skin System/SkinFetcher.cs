using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinFetcher : MonoBehaviour
{
    public string[] skinNames;
    public GameObject[] skinPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        UserSettings.CheckDefaults();
        int skinIndex = Array.IndexOf(skinNames, PlayerPrefs.GetString("Skin"));
        if (skinIndex == -1) { skinIndex = 0; }

        TopManager.skinPrefab = skinPrefabs[skinIndex];

        foreach (SkinApplier applier in FindObjectsOfType<SkinApplier>())
        {
            applier.ApplySkin();
        }

    }


}
