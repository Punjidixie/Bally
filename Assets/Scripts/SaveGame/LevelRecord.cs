using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRecord
{
    public string sceneName;
    public bool unlocked;
    public bool completed;
    public int stars;
    public int crystals;
    public float time;
    public float threeStarTime;

    // THIS DOES NOT SET THE SCENENAME
    public static LevelRecord GetDefault()
    {
        LevelRecord result = new LevelRecord
        {
            unlocked = false,
            completed = false,
            stars = 0,
            crystals = 0,
            time = Mathf.Infinity,
            threeStarTime = Mathf.Infinity
        };

        return result;
    }

    public static LevelRecord CombineRecords(LevelRecord levelRecord1, LevelRecord levelRecord2)
    {
        LevelRecord combination = new LevelRecord
        {
            sceneName = levelRecord1.sceneName,
            unlocked = levelRecord1.unlocked || levelRecord2.unlocked,
            completed = levelRecord1.completed || levelRecord2.completed,
            stars = Mathf.Max(levelRecord1.stars, levelRecord2.stars),
            crystals = Mathf.Max(levelRecord1.crystals, levelRecord2.crystals),
            time = Mathf.Min(levelRecord1.time, levelRecord2.time),
            threeStarTime = Mathf.Min(levelRecord1.threeStarTime, levelRecord2.threeStarTime)
        };

        return combination;
    }

    
}
