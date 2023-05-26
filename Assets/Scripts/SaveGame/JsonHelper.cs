using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonHelper
{

    public static string recordPath = Path.Combine(Application.persistentDataPath, "Game", "LevelRecords");
    public static string allRecordPath = Path.Combine(Application.persistentDataPath, "Game");

    // Reads and returns a level record object. If not found, returns default.
    public static LevelRecord GetLevelRecord(string sceneName)
    {
        string path = Path.Combine(recordPath, sceneName);
        string json = "";

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }
            return JsonUtility.FromJson<LevelRecord>(json);
        }
        else
        {
            LevelRecord result = LevelRecord.GetDefault();
            result.sceneName = sceneName;

            return result;
        } 
    }

    // Combines an old and a new record, then writes it to a file
    public static void WriteLevelRecord(LevelRecord levelRecord)
    {
        string path = Path.Combine(recordPath, levelRecord.sceneName);

        LevelRecord previousRecord = GetLevelRecord(levelRecord.sceneName);

        string json = JsonUtility.ToJson(LevelRecord.CombineRecords(previousRecord, levelRecord));

        Directory.CreateDirectory(recordPath);
        FileStream fileStream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }

        Debug.Log("WRITTEN RECORD TO " + path);

    }

    public static AllRecord GetAllRecord()
    {
        string path = Path.Combine(allRecordPath, "AllRecord");
        string json = "";

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }
            return JsonUtility.FromJson<AllRecord>(json);
        }
        else
        {
            AllRecord result = AllRecord.GetDefault();
            return result;
        }
    }

    public static void AddStarToAllRecord(int increment)
    {
        string path = Path.Combine(allRecordPath, "AllRecord");

        AllRecord record = GetAllRecord();
        record.totalStars += increment;

        string json = JsonUtility.ToJson(record);

        Directory.CreateDirectory(recordPath);
        FileStream fileStream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }

        Debug.Log("WRITTEN ALLRECORD TO " + path);
    }
}
