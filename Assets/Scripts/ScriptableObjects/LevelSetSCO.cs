using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSetSCO", menuName = "ScriptableObjects/LevelSetSCO", order = 1)]
public class LevelSetSCO : ScriptableObject
{
    public string setName;
    public string setId;
    public bool consececutiveUnlock;
    public int requiredStars;
    public LevelSCO[] levels;
    
}
