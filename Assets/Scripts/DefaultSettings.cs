using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DefaultSettings
{
    public static Vector3 tiltCalibration; //offset
    public static string movementMode = "Joystick"; // Keyboard Joystick DeviceTilting
    public static float mouseSensitivity = 10;
    public static float dragSensitivity = 0.6f;
    public static float cameraAngularSpeed = 150; //degrees per sec



    public static void CalibrateTilt()
    {
        tiltCalibration = Input.acceleration;
    }

    public static int GetRequiredStars(string setName)
    {
        switch (setName)
        {
            case "Level":
                return 0;

            case "AfterLVS":
                return 0;

            case "FinalBeta":
                return 40;

            default:
                return 0;
        }
        
    }

    public static int GetNumberOfLevels(string setName)
    {
        switch (setName)
        {
            case "Level":
                return 20;

            case "AfterLVS":
                return 20;

            case "FinalBeta":
                return 20;

            default:
                return 20;
        }

    }
}
