using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BetaSelController : MonoBehaviour
{
    public GameObject ball;
    public Joystick joystick;
    
    public FixedTouchField scrollArea;
    public GameObject calibrateButton;
    public Dropdown dropdown;
    public Dropdown dropdown2;
    public Slider sensitivitySlider;
    public GameObject sensitivityPanel;
    public GameObject resetConfirmPanel;
  
    // Start is called before the first frame update
    void Start()
    {
        
        UserSettings.CheckDefaults(); //so I can use UserSettings instead of PlayerPrefs every frame (this is maybe faster)
        Time.timeScale = 1;
        //sensitivitySlider.value = UserSettings.dragSensitivity;
        sensitivitySlider.value = PlayerPrefs.GetFloat("DragSensitivity");
        //load from UserSettings
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 0 = Joystick
    // 1 = DeviceTilting
    // 2 = Keyboard
    public void HandleDropdownInput(int i)
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

    public void HandleDropdownInput2(int i)
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

        sensitivityPanel.SetActive(true);

        PlayerPrefs.SetString("MovementMode", "Joystick");
    }

    public void ToDeviceTilting()
    {
        joystick.gameObject.SetActive(false);
        
        calibrateButton.SetActive(true);

        sensitivityPanel.SetActive(true);

        PlayerPrefs.SetString("MovementMode", "DeviceTilting");
    }

    public void ToKeyboard()
    {
        joystick.gameObject.SetActive(false);
        
        calibrateButton.SetActive(false);

        sensitivityPanel.SetActive(false);

        PlayerPrefs.SetString("MovementMode", "Keyboard");
    }

    public void LoadLevel(string setName)
    {
        SceneManager.LoadScene(setName);
    }

    public void CalibrateTilt()
    {
        UserSettings.tiltCalibration = Input.acceleration;
    }

    public void ChangeDragSensitivity(float s)
    {
        PlayerPrefs.SetFloat("DragSensitivity", s);
    }

    public void ResetEverything()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BringUpResetConfirmPanel()
    {
        resetConfirmPanel.SetActive(true);
    }

    public void BringDownResetConfirmPanel()
    {
        resetConfirmPanel.SetActive(false);
    }

}
