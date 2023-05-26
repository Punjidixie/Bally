using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TopManager : MonoBehaviour
{

    public static int levelIndex;
    public static LevelSetSCO levelSetSCO;

    public static bool instantiated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instantiated)
        {
            Destroy(gameObject);
            return;
        }

        else { instantiated = true;  }

        DontDestroyOnLoad(gameObject);
    }

    public static void StartSet()
    {
        SceneManager.LoadScene(levelSetSCO.levels[levelIndex].sceneName);
    }

    public static void NextLevel()
    {
        if (levelSetSCO)
        {
            if (levelIndex + 1 < levelSetSCO.levels.Length)
            {
                levelIndex++;
                SceneManager.LoadScene(levelSetSCO.levels[levelIndex].sceneName);
            }
        }
    }

    public static void CompleteCurrentLevel(LevelRecord levelRecord)
    {
        int starIncrement = Mathf.Clamp(levelRecord.stars - JsonHelper.GetLevelRecord(levelRecord.sceneName).stars, 0, 3);

        JsonHelper.AddStarToAllRecord(starIncrement);
        JsonHelper.WriteLevelRecord(levelRecord);

        if (levelSetSCO.consececutiveUnlock && levelIndex < levelSetSCO.levels.Length - 1)
        {
            string nextSceneName = levelSetSCO.levels[levelIndex + 1].sceneName;
            LevelRecord freshRec = JsonHelper.GetLevelRecord(nextSceneName);
            freshRec.unlocked = true;
            JsonHelper.WriteLevelRecord(freshRec);
        }
    }

    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
