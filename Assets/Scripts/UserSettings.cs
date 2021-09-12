using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserSettings
{
    public static Vector3 tiltCalibration; //offset
    public static string movementMode; // Keyboard Joystick DeviceTilting
    public static float mouseSensitivity;
    public static float dragSensitivity;
    public static float cameraAngularSpeed; //degrees per sec

    public static bool cheat = true;

    public static void CalibrateTilt()
    {
        tiltCalibration = Input.acceleration;
    }

    public static void LoadControlsFromPlayerPrefs()
    {
        Application.targetFrameRate = 60;
        movementMode = PlayerPrefs.GetString("MovementMode", DefaultSettings.movementMode);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", DefaultSettings.mouseSensitivity);
        dragSensitivity = PlayerPrefs.GetFloat("DragSensitivity", DefaultSettings.dragSensitivity);
    }
}
