using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleController : MonoBehaviour
{

    //movement inputs
    public Joystick joystick;
    public FixedTouchField scrollArea; //panel

    public GameObject calibrateButton;
    public Dropdown dropdown;
    public Dropdown dropdown2;
    public Dropdown dropdown3;

    public Slider mouseSensitivitySlider;
    public GameObject mouseSensitivityObj;

    public Slider dragSensitivitySlider;
    public GameObject dragSensitivityObj;

    public GameObject toggleCursorPanel;

    // Start is called before the first frame update
    void Start()
    {
        UserSettings.CheckDefaults();

        LoadFromPlayerPrefs();
        
    }

    private void Update()
    {
        switch (PlayerPrefs.GetString("MovementMode"))
        {
            case "Keyboard":
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Cursor.visible = !Cursor.visible;
                }
                break;
        }
    }

    public void LoadFromPlayerPrefs()
    {
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        dragSensitivitySlider.value = PlayerPrefs.GetFloat("DragSensitivity");

        switch (PlayerPrefs.GetString("MovementMode"))
        {
            case "Joystick":
                ToJoystick();
                dropdown.value = 0;
                break;
            case "DeviceTilting":
                ToDeviceTilting();
                dropdown.value = 1;
                break;
            case "Keyboard":
                ToKeyboard();
                dropdown.value = 2;
                break;
            default:
                ToJoystick();
                dropdown.value = 0;
                break;
        }

        switch (PlayerPrefs.GetString("CameraMode"))
        {
            case "Manual":
                ToManualCam();
                dropdown2.value = 0;
                break;
            case "Auto":
                ToAutoCam();
                dropdown2.value = 1;
                break;
            default:
                ToManualCam();
                dropdown2.value = 0;
                break;
        }

        Dictionary<float, int> fpsDict = new Dictionary<float, int>()
        {

            [-1f] = 0,
            [30f] = 1,
            [60f] = 2,
            [120f] = 3

        };
        Application.targetFrameRate = (int)PlayerPrefs.GetFloat("FPS");
        dropdown3.value = fpsDict[PlayerPrefs.GetFloat("FPS")];
        
    }

    public void ChangeDragSensitivity(float s)
    {
        PlayerPrefs.SetFloat("DragSensitivity", s);
    }

    public void ChangeMouseSensitivity(float s)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", s);
    }

    public void SetMovementMode(int i)
    {
        switch (i)
        {
            case 0:
                ToJoystick();
                break;
            case 1:
                ToDeviceTilting();
                break;
            case 2:
                ToKeyboard();
                break;
            default:
                ToJoystick();
                break;
        }
    }

    public void SetCameraMode(int i)
    {
        switch (i)
        {
            case 0:
                ToManualCam();
                break;
            case 1:
                ToAutoCam();
                break;
            default:
                ToManualCam();
                break;
        }
    }

    public void SetFPS(int i)
    {
        int[] fpsArray = { -1, 30, 60, 120 };
        PlayerPrefs.SetFloat("FPS", fpsArray[i]);
        Application.targetFrameRate = fpsArray[i];
    }

    public void ToAutoCam()
    {
        PlayerPrefs.SetString("CameraMode", "Auto");
    }

    public void ToManualCam()
    {
        PlayerPrefs.SetString("CameraMode", "Manual");
    }

    public void ToJoystick()
    {
        joystick.gameObject.SetActive(true);

        calibrateButton.SetActive(false);

        mouseSensitivityObj.SetActive(false);

        dragSensitivityObj.SetActive(true);

        toggleCursorPanel.SetActive(false);

        PlayerPrefs.SetString("MovementMode", "Joystick");
    }

    public void ToDeviceTilting()
    {
        joystick.gameObject.SetActive(false);

        calibrateButton.SetActive(true);

        mouseSensitivityObj.SetActive(false);

        dragSensitivityObj.SetActive(true);

        toggleCursorPanel.SetActive(false);

        PlayerPrefs.SetString("MovementMode", "DeviceTilting");
    }

    public void ToKeyboard()
    {
        joystick.gameObject.SetActive(false);

        calibrateButton.SetActive(false);

        mouseSensitivityObj.SetActive(true);

        dragSensitivityObj.SetActive(false);

        toggleCursorPanel.SetActive(true);

        PlayerPrefs.SetString("MovementMode", "Keyboard");
    }

    public void RevertToDefault()
    {
        UserSettings.RevertDefaults();
        LoadFromPlayerPrefs();
    }

    public void CalibrateTilt()
    {
        UserSettings.tiltCalibration = Input.acceleration;
    }

    public void Back()
    {
        SceneManager.LoadScene("Level Select");
    }
}
