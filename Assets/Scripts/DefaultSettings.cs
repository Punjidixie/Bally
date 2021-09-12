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
}
