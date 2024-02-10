using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelController : MonoBehaviour
{
    public float maxTime;

    //crystal information
    public float reqCrystal1;
    public float reqCrystal2;
    public float reqCrystal3;
    public float fallOutHeight;

    public string levelName;


    [System.NonSerialized] public string setName;
    [System.NonSerialized] public int levelNumber;
    
    Tilting tilting;

    //panning after intro
    Quaternion cameraStartRotation;
    Vector3 cameraStartPosition;
    Coroutine whilePanningRoutine;

    //clock
    public GameObject timer;
    public Text timeText;
    public Text timeTextMs;

    //intro panel
    public GameObject introPanel;
    public TMP_Text setNameText;
    public TMP_Text levelNumberText;
    public TMP_Text levelNameText;

    //countdown
    public GameObject countDownPanel;
    public Text countDownText;

    //crystal tracker
    public CrystalTracker crystalTracker;

    //status effect tracker
    public StatusEffectTracker statusEffectTracker;

    //win panel
    public WinPanel winPanel;

    //lose panel
    public GameObject losePanel;
    public TMP_Text losingMessage;

    //movement inputs
    public Joystick joystick;
    public FixedTouchField scrollArea; //panel
   
    //pausing
    public GameObject pauseButton;
    public GameObject pausePanel;

    //calibrate button
    public GameObject calibrateButton;

    //name text
    public GameObject namePanel;
    public TMP_Text nameText;

    //debug
    public Text speedText;
    public Text framerateText;

    [HideInInspector]
    public float timePassed;

    [HideInInspector]
    public int crystals;

    [HideInInspector]
    public string levelState; // Intro -> Panning -> InGame -> Winning -> End

    // Start is called before the first frame update
    void Start()
    {
        //The FPS is not used here. It will always be default if not run from the level select.
        UserSettings.CheckDefaults();

        Time.timeScale = 1;
        levelState = "Intro";

        cameraStartPosition = Camera.main.transform.position;
        cameraStartRotation = Camera.main.transform.rotation;

        tilting = FindObjectOfType<Tilting>();

        timePassed = 0;

        crystals = 0;

        LoadTopManagerInfo();
        nameText.text = setName + " " + levelNumber.ToString();
        levelNameText.text = levelName;


        if (reqCrystal1 == 0)
        {
            foreach (GoodCube winBox in FindObjectsOfType<GoodCube>())
            {
                winBox.active = true;
                winBox.winLight.SetActive(true);
            }
        }

        StartCoroutine(FramerateCounterUpdate());
       
    }

    void LoadTopManagerInfo()
    {
        if (TopManager.instantiated)
        {
            levelNumber = TopManager.levelIndex + 1;
            setName = TopManager.levelSetSCO.setName;

            levelNumberText.text = "Level " + levelNumber.ToString();
            setNameText.text = setName;
        }
        
    }
   
    IEnumerator FramerateCounterUpdate()
    {
        int framesPassed = 0;
        float accumulatedTime = 0f;
        while (true)
        {
            accumulatedTime += Time.unscaledDeltaTime;
            framesPassed++;
            if (framesPassed == 5)
            {
                framerateText.text = "FPS: " + Mathf.Round(framesPassed / accumulatedTime * 100) / 100f;
                accumulatedTime = 0;
                framesPassed = 0;
            }
            
            yield return null;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            UpdateRuntime();
        }
        else
        {
            UpdateEditor();
        }
    }

    void UpdateEditor()
    {
        levelNameText.text = levelName;
    }

    void UpdateRuntime()
    {
        speedText.text = "Speed: " + Mathf.Round(tilting.GetComponent<Rigidbody>().velocity.magnitude * 100) / 100f;
        //previousSpeed = ball.GetComponent<Rigidbody>().velocity;
        
        switch (levelState)
        {
            case "Intro":
                if (Input.anyKeyDown)
                {
                    whilePanningRoutine = StartCoroutine(WhilePanning());
                }
                break;
            case "Panning":
                if (Input.anyKeyDown)
                {
                    StartCoroutine(InGameRoutine());
                    StopCoroutine(whilePanningRoutine);
                }
                break;
            case "InGame":
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }
                else if (timePassed >= maxTime)
                {
                    StartCoroutine(TimesUpRoutine());
                }
                else if (tilting.transform.position.y <= fallOutHeight)
                {
                    StartCoroutine(FallOutRoutine());
                }
                else
                {
                    if (!tilting.isGhostMode) { timePassed += Time.deltaTime; }
                    UpdateClock();
                }
                break;
            case "Paused":
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ContinueGame();
                }
                break;
            default:
                break;
        }
        
    }

    void UpdateClock()
    {
        //update clock time
        float minutes = Mathf.Floor((maxTime - timePassed) / 60f);
        float seconds = Mathf.Floor((maxTime - timePassed) - 60 * minutes);
        float millisecs = Mathf.Floor((maxTime - timePassed - 60 * minutes - seconds) * 100);

        string minutesString = minutes.ToString();
        string secondsString;
        string millisecsString;
        if (seconds < 10)
        {
            secondsString = "0" + seconds.ToString();
        }
        else
        {
            secondsString = seconds.ToString();
        }
        if (millisecs < 10)
        {
            millisecsString = "0" + millisecs.ToString();
        } 
        else
        {
            millisecsString = millisecs.ToString();
        }
        timeText.text = minutesString + ":" + secondsString;
        timeTextMs.text = ":" + millisecsString;
    }
    
    public IEnumerator WinRoutine() //called from Tilting.cs, setup for Winning state
    {
        
        levelState = "Winning";
        
        HideEssentials();
        HideControls();

        winPanel.gameObject.SetActive(true);
        

        string sceneName = SceneManager.GetActiveScene().name;
        LevelRecord previousRecord = JsonHelper.GetLevelRecord(sceneName);

       winPanel.SetWinInfo(previousRecord);

        LevelRecord thisRecord = new LevelRecord
        {
            sceneName = sceneName,
            unlocked = true,
            completed = true,
            stars = crystalTracker.stars,
            crystals = crystals,
            time = timePassed,
            threeStarTime = (crystalTracker.stars == 3) ? timePassed : Mathf.Infinity
        };

        if (TopManager.instantiated)
        {
            //This will save the record.
            TopManager.CompleteCurrentLevel(thisRecord);
        }

        yield return new WaitForSecondsRealtime(1.5f);
        //levelState = "End";
        Time.timeScale = 0;
        PlayerPrefs.Save();
    }

   
    
    public IEnumerator LoseRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);
    }

    public IEnumerator WhilePanning()
    {
        introPanel.SetActive(false);
        countDownPanel.SetActive(true);
        levelState = "Panning";
        float t = 0;

        while (t < 3)
        {
            countDownText.text = Mathf.Ceil(3 - t).ToString();
            t += Time.deltaTime;

            float h = tilting.lengthToCamera * Mathf.Cos(tilting.xRotationOffset * Mathf.Deg2Rad) - tilting.heightToCamera * Mathf.Sin(tilting.xRotationOffset * Mathf.Deg2Rad);
            float v = tilting.heightToCamera * Mathf.Cos(tilting.xRotationOffset * Mathf.Deg2Rad) + tilting.lengthToCamera * Mathf.Sin(tilting.xRotationOffset * Mathf.Deg2Rad);
            Camera.main.transform.position = Vector3.Lerp(cameraStartPosition, tilting.transform.position + new Vector3(0, v, -h), t / 3f);
            Camera.main.transform.rotation = Quaternion.Lerp(cameraStartRotation, Quaternion.Euler(tilting.xRotationOffset, 0, 0), t / 3f);
            Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);
            yield return null;
        }
        StartCoroutine(InGameRoutine());

    }

    //setup for going to InGame state
    public IEnumerator InGameRoutine()
    {
        StopCoroutine(WhilePanning());
        tilting.GetComponent<Rigidbody>().useGravity = true;
        float h = tilting.lengthToCamera * Mathf.Cos(tilting.xRotationOffset * Mathf.Deg2Rad) - tilting.heightToCamera * Mathf.Sin(tilting.xRotationOffset * Mathf.Deg2Rad);
        float v = tilting.heightToCamera * Mathf.Cos(tilting.xRotationOffset * Mathf.Deg2Rad) + tilting.lengthToCamera * Mathf.Sin(tilting.xRotationOffset * Mathf.Deg2Rad);
        Camera.main.transform.position = tilting.transform.position + new Vector3(0, v, -h);
        Camera.main.transform.rotation = Quaternion.Euler(tilting.xRotationOffset, 0, 0);

        BringUpEssentials();
        BringUpControls();
        
        timePassed = 0;
        Time.timeScale = 1;
        levelState = "InGame";
        countDownText.text = "Start!";

        yield return new WaitForSecondsRealtime(0.5f);
        countDownPanel.SetActive(false);
        
        
    }

    public void RestartLevel()
    {

        TopManager.RestartLevel();
        
    }

    public void ToLevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }

    public void NextLevel()
    {

        TopManager.NextLevel();
        
    }

    public IEnumerator TimesUpRoutine()
    {

        HideEssentials();
        HideControls();

        levelState = "End";
        losePanel.SetActive(true);
        losingMessage.text = "Out of time...";
        tilting.GetComponent<Tilting>().UpdateCamera();
        Time.timeScale = 0;
       
        yield return new WaitForSecondsRealtime(0);
    }

    public IEnumerator FallOutRoutine()
    {

        HideEssentials();
        HideControls();

        levelState = "Losing";
        losePanel.SetActive(true);
        losingMessage.text = "Fall out...";

        yield return new WaitForSecondsRealtime(2f);
        tilting.GetComponent<Tilting>().UpdateCamera();
        Time.timeScale = 0;
        levelState = "End";
    }

    public IEnumerator DieRoutine()
    {

        HideEssentials();
        HideControls();

        levelState = "Losing";
        losePanel.SetActive(true);
        losingMessage.text = "Destroyed...";

        yield return new WaitForSecondsRealtime(4f);
        tilting.GetComponent<Tilting>().UpdateCamera();
        Time.timeScale = 0;
        levelState = "End";
    }

    public void CalibrateTilt()
    {
        UserSettings.tiltCalibration = Input.acceleration;
    }

    public void PauseGame()
    {
        HideEssentials();
        HideControls();

        tilting.GetComponent<Tilting>().LateUpdateInGame();
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        levelState = "Paused";
    }

    public void ContinueGame()
    {
        BringUpEssentials();
        BringUpControls();

        pausePanel.SetActive(false);
        Time.timeScale = 1;
        levelState = "InGame";
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && levelState == "InGame" && !Application.isEditor)
        {
            PauseGame();
        }
    }

    public void BringUpEssentials()
    {
        timer.SetActive(true);
        statusEffectTracker.gameObject.SetActive(true);
        crystalTracker.gameObject.SetActive(true);
        namePanel.SetActive(true);
    }

    public void HideEssentials()
    {
        timer.SetActive(false);
        statusEffectTracker.gameObject.SetActive(false);
        crystalTracker.gameObject.SetActive(false);
        namePanel.SetActive(false);
    }

    public void BringUpControls()
    {
        switch (PlayerPrefs.GetString("MovementMode"))
        {
            case "DeviceTilting":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                joystick.gameObject.SetActive(false);
                calibrateButton.SetActive(true);
                break;
            case "Joystick":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                joystick.gameObject.SetActive(true);
                calibrateButton.SetActive(false);
                break;
            case "Keyboard":
                Cursor.visible = false;
                pauseButton.SetActive(false);
                joystick.gameObject.SetActive(false);
                calibrateButton.SetActive(false);
                break;
            default:
                break;
        }
    }
    public void HideControls()
    {
        Cursor.visible = true;
        pauseButton.SetActive(false);
        joystick.gameObject.SetActive(false);
        calibrateButton.SetActive(false);
    }
}


