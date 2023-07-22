using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserSettings
{
    public static Vector3 tiltCalibration; //offset

    public static bool cheat = true;

    static Dictionary<string, string> defaultStr = new Dictionary<string, string>() {

        ["MovementMode"] = "Joystick",
        ["CameraMode"] = "Manual",
        ["Skin"] = "WoodFace"

    };

    static Dictionary<string, float> defaultFloat = new Dictionary<string, float>()
    {

        ["MouseSensitivity"] = 10f,
        ["DragSensitivity"] = 0.2f,
        ["FPS"] = -1f

    };

    public static void CalibrateTilt()
    {
        tiltCalibration = Input.acceleration;
    }

    // Check player prefs, set default value if key not present
    public static void CheckDefaults()
    {

        foreach (KeyValuePair<string, string> entry in defaultStr)
        {
            if (!PlayerPrefs.HasKey(entry.Key))
            {
                PlayerPrefs.SetString(entry.Key, entry.Value);
            }
            
        }

        foreach (KeyValuePair<string, float> entry in defaultFloat)
        {
            if (!PlayerPrefs.HasKey(entry.Key))
            {
                PlayerPrefs.SetFloat(entry.Key, entry.Value);
            }
        }
    }

    public static void RevertDefaults()
    {

        foreach (KeyValuePair<string, string> entry in defaultStr)
        {
            PlayerPrefs.SetString(entry.Key, entry.Value);

        }

        foreach (KeyValuePair<string, float> entry in defaultFloat)
        {
            PlayerPrefs.SetFloat(entry.Key, entry.Value);
        }
    }
}
