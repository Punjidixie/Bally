using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public float maxTime;
    public float reqCrystal1;
    public float reqCrystal2;
    public float reqCrystal3;
    public float fallOutHeight;

    public string setName;
    public int levelNumber;
    
    public GameObject ball;
    Tilting tilting;

    
    float cameraPanSpeed;
    public float cameraPanTime;
    Vector3 toTopOfBall;
    Vector3 toResetRotation;

    Quaternion cameraStartRotation;
    Vector3 cameraStartPosition;
    Coroutine whilePanningRoutine;

    //clock
    public GameObject timer;
    public Text timeText;
    public Text timeTextMs;

    //intro panel
    public GameObject introPanel;

    //countdown
    public GameObject countDownPanel;
    public Text countDownText;

    //crystal panel
    public GameObject crystalsCount;
    public Text crystalsText;

    //crystal tracker
    public CrystalTracker crystalTracker;

    //status effect tracker
    public StatusEffectTracker statusEffectTracker;

    //win panel
    public GameObject winPanel;
    public Text winningMessage;
    public Text timeTakenText;
    public Text timeTakenTextMs;
    public Text crystalsWinText;
    public GameObject winStar1;
    public GameObject winStar2;
    public GameObject winStar3;
    public Text bestTimeMessage;
    public Text bestCrystalMessage;

    //lose panel
    public GameObject losePanel;
    public Text losingMessage;

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
    public Text nameText;

    public Text speedText;
    public Vector3 previousSpeed;
    public Text framerateText;
    
    float timePassed;
    public int crystals;
    public string levelState; // Intro -> Panning -> InGame -> Winning -> End

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        UserSettings.LoadControlsFromPlayerPrefs();

        Time.timeScale = 1;
        levelState = "Intro";

        cameraStartPosition = Camera.main.transform.position;
        cameraStartRotation = Camera.main.transform.rotation;

        tilting = ball.GetComponent<Tilting>();

        toTopOfBall = ball.transform.position + new Vector3(0, 1.5f, -tilting.distanceToCameraNormal) - Camera.main.transform.position;
        cameraPanTime = 3;
        cameraPanSpeed = toTopOfBall.magnitude / cameraPanTime;
        timePassed = 0;

        crystals = 0;

        nameText.text = setName + " " + levelNumber.ToString();

        //crystalsText.text = "Crystals: " + 0 + "/" + reqCrystal1.ToString();

        if (reqCrystal1 == 0)
        {
            foreach (GameObject winBox in GameObject.FindGameObjectsWithTag("WinBox"))
            {
                winBox.GetComponent<GoodCube>().active = true;
            }
        }

        CalculateToResetRotation();
        StartCoroutine(FramerateCounterUpdate());
       
    }
    void CalculateToResetRotation()
    {
        float xFixed = Camera.main.transform.eulerAngles.x - 0;
        float yFixed = Camera.main.transform.eulerAngles.y - 0;
        float zFixed = Camera.main.transform.eulerAngles.z - 0;
        if (xFixed > 180) { xFixed -= 360; }
        if (yFixed > 180) { yFixed -= 360; }
        if (zFixed > 180) { zFixed -= 360; }
        toResetRotation = -new Vector3(xFixed, yFixed, zFixed) / cameraPanTime;
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
        speedText.text = "Speed: " + Mathf.Round(ball.GetComponent<Rigidbody>().velocity.magnitude * 100) / 100f;
        //previousSpeed = ball.GetComponent<Rigidbody>().velocity;
        

        timePassed += Time.deltaTime;
        
        if (levelState == "Intro")
        {
            //Camera.main.transform.LookAt(ball.transform.position + new Vector3(0, 1.5f, 10));
            if (Input.anyKeyDown)
            {

                whilePanningRoutine = StartCoroutine(WhilePanning());
                
            }
        }
        else if (levelState == "Panning")
        {
            //countDownText.text = Mathf.Ceil(cameraPanTime - timePassed).ToString();
            if (Input.anyKeyDown)
            {
                
                StartCoroutine(InGameRoutine());
                StopCoroutine(whilePanningRoutine);
            }
            else
            {
                //CameraPanToTopOfBall();
            }
        }
        

        else if (levelState == "InGame")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
            else if (timePassed >= maxTime)
            {
                StartCoroutine(TimesUpRoutine());
            }
            else if (ball.transform.position.y <= fallOutHeight)
            {
                StartCoroutine(FallOutRoutine());
            }
            else
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
            
        }

        else if (levelState == "Paused")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ContinueGame();
            }
        }

        
        
        


    }

    void CameraPanToTopOfBall()
    {
        //destination
        toTopOfBall = ball.transform.position + new Vector3(0, 1.5f, -tilting.distanceToCameraNormal) - Camera.main.transform.position;

        Camera.main.transform.position += toTopOfBall.normalized * cameraPanSpeed * Time.deltaTime;

        Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x + (toResetRotation.x * Time.deltaTime), Camera.main.transform.eulerAngles.y + (toResetRotation.y * Time.deltaTime), Camera.main.transform.eulerAngles.z + (toResetRotation.z * Time.deltaTime));
        //Camera.main.transform.LookAt(ball.transform.position + new Vector3(0, 1.5f, 10));
        
        if (toTopOfBall.magnitude < cameraPanSpeed * Time.deltaTime)
        {
            Camera.main.transform.position = ball.transform.position + new Vector3(0, 1.5f, -tilting.distanceToCameraNormal);
            
            StartCoroutine(InGameRoutine());
        }
    }

    
    public IEnumerator WinRoutine() //called from Tilting.cs, setup for Winning state
    {
        StartCoroutine(WinCrystalStarAnimation());
        PlayerPrefs.SetInt(setName + levelNumber + "Completed", 1);
        Cursor.visible = true;
        levelState = "Winning";
        pauseButton.SetActive(false);
        calibrateButton.SetActive(false);
        timer.SetActive(false);
        statusEffectTracker.gameObject.SetActive(false);
        crystalTracker.gameObject.SetActive(false);
        namePanel.SetActive(false);

        joystick.gameObject.SetActive(false);


        winPanel.SetActive(true);
        winningMessage.text = "Level Completed!\n" + setName + " " + levelNumber.ToString();

        //calculate time taken
        float minutes = Mathf.Floor(timePassed / 60f);
        float seconds = Mathf.Floor(timePassed - 60 * minutes);
        float millisecs = Mathf.Floor((timePassed - 60 * minutes - seconds) * 100);

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
        timeTakenText.text = minutesString + ":" + secondsString;
        timeTakenTextMs.text = ":" + millisecsString;

        //new record
        if (crystals > PlayerPrefs.GetInt(setName + levelNumber + "BestCrystal", -1))
        {
            bestCrystalMessage.gameObject.SetActive(true);
            PlayerPrefs.SetInt(setName + levelNumber + "BestCrystal", crystals);
        }

        if (crystalTracker.stars > PlayerPrefs.GetInt(setName + levelNumber + "BestStar", 0))
        {
            int oldStars = PlayerPrefs.GetInt(setName + levelNumber + "BestStar", 0);
            PlayerPrefs.SetInt(setName + levelNumber + "BestStar", crystalTracker.stars);
            PlayerPrefs.SetInt("TotalStar", PlayerPrefs.GetInt("TotalStar", 0) - oldStars + crystalTracker.stars);

        }

        bool hasBestTime = timePassed < PlayerPrefs.GetFloat(setName + levelNumber + "BestTime", Mathf.Infinity);
        bool hasBest3sTime = timePassed < PlayerPrefs.GetFloat(setName + levelNumber + "Best3sTime", Mathf.Infinity) && crystalTracker.stars == 3;

        if (hasBestTime && hasBest3sTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best time & 3-star time!";
            PlayerPrefs.SetFloat(setName + levelNumber + "BestTime", timePassed);
            PlayerPrefs.SetFloat(setName + levelNumber + "Best3sTime", timePassed);
        }
        else if (hasBestTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best time!";
            PlayerPrefs.SetFloat(setName + levelNumber + "BestTime", timePassed);
        }
        else if (hasBest3sTime)
        {
            bestTimeMessage.gameObject.SetActive(true);
            bestTimeMessage.text = "New best 3-star time!";
            PlayerPrefs.SetFloat(setName + levelNumber + "Best3sTime", timePassed);
        }
        //crystalsWinText.text = crystals.ToString();
        ////calculate stars
        //switch (crystalTracker.stars)
        //{
        //    case 0:
        //        break;
        //    case 1:
        //        winStar1.GetComponent<Image>().color = crystalTracker.yellow;
        //        break;
        //    case 2:
        //        winStar1.GetComponent<Image>().color = crystalTracker.yellow;
        //        winStar2.GetComponent<Image>().color = crystalTracker.yellow;
        //        break;
        //    case 3:
        //        winStar1.GetComponent<Image>().color = crystalTracker.yellow;
        //        winStar2.GetComponent<Image>().color = crystalTracker.yellow;
        //        winStar3.GetComponent<Image>().color = crystalTracker.yellow;
        //        break;
        //}
        yield return new WaitForSecondsRealtime(1.5f);
        //levelState = "End";
        Time.timeScale = 0;
        PlayerPrefs.Save();
    }
    public IEnumerator WinCrystalStarAnimation()
    {
        float t = 0;
        float spamRate = 30f; //crystals per second
        int currentStar = 0;
        while (t <= 1)
        {
            t += Time.unscaledDeltaTime / (crystals / spamRate);
            float number = Mathf.Round(Mathf.Lerp(0, crystals, t));
            crystalsWinText.text = number.ToString();

            if (number >= reqCrystal1 && currentStar < 1)
            {
                winStar1.GetComponent<Image>().color = crystalTracker.yellow;
                winStar1.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 1;
            }
            if (number >= reqCrystal2 && currentStar < 2)
            {
                winStar2.GetComponent<Image>().color = crystalTracker.yellow;
                winStar2.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 2;
            }
            if (number >= reqCrystal3 && currentStar < 3)
            {
                winStar3.GetComponent<Image>().color = crystalTracker.yellow;
                winStar3.GetComponent<Animator>().SetTrigger("Boop");
                currentStar = 3;
            }
            yield return null;
        }
        crystalsWinText.text = crystals.ToString();
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
        timePassed = 0;
        float t = 0;

        while (t < 3)
        {
            countDownText.text = Mathf.Ceil(3 - t).ToString();
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(cameraStartPosition, ball.transform.position + new Vector3(0, tilting.heightToCamera, -tilting.distanceToCameraNormal), t / 3f);
            Camera.main.transform.rotation = Quaternion.Lerp(cameraStartRotation, Quaternion.Euler(0, 0, 0), t / 3f);
            Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);
            yield return null;
        }
        StartCoroutine(InGameRoutine());

    }
    //setup for going to InGame state
    public IEnumerator InGameRoutine()
    {
        StopCoroutine(WhilePanning());
        Camera.main.transform.position = ball.transform.position + new Vector3(0, tilting.heightToCamera, -tilting.distanceToCameraNormal);
        Camera.main.transform.LookAt(ball.transform.position + new Vector3(0, tilting.heightToCamera, 10));

        timer.SetActive(true);
        crystalTracker.gameObject.SetActive(true);
        namePanel.SetActive(true);
        
        switch (PlayerPrefs.GetString("MovementMode", DefaultSettings.movementMode))
        {
            case "DeviceTilting":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                calibrateButton.SetActive(true);
                break;
            case "Joystick":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                
                joystick.gameObject.SetActive(true);
                break;
            case "Keyboard":
                Cursor.visible = false;
                break;
            default:
                break;
        }
        
        timePassed = 0;
        Time.timeScale = 1;
        levelState = "InGame";
        countDownText.text = "Start!";

        yield return new WaitForSecondsRealtime(0.5f);
        countDownPanel.SetActive(false);
        
        
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
    public void RestartLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void NextLevel()
    {

        
        SceneManager.LoadScene(setName + (levelNumber + 1));
        
    }

    public IEnumerator TimesUpRoutine()
    {
        pauseButton.SetActive(false);
        calibrateButton.SetActive(false);
        timer.SetActive(false);
        statusEffectTracker.gameObject.SetActive(false);
        crystalTracker.gameObject.SetActive(false);
        namePanel.SetActive(false);

        joystick.gameObject.SetActive(false);
        

        Cursor.visible = true;
        levelState = "End";
        losePanel.SetActive(true);
        losingMessage.text = "Time's up";
        Time.timeScale = 0;
       
        yield return new WaitForSecondsRealtime(0);
    }

    public IEnumerator FallOutRoutine()
    {
        pauseButton.SetActive(false);
        calibrateButton.SetActive(false);
        timer.SetActive(false);
        statusEffectTracker.gameObject.SetActive(false);
        crystalTracker.gameObject.SetActive(false);
        namePanel.SetActive(false);

        joystick.gameObject.SetActive(false);
        

        Cursor.visible = true;
        levelState = "Losing";

        losePanel.SetActive(true);
        losingMessage.text = "Fall out";
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 0;
        levelState = "End";
    }

    public void CalibrateTilt()
    {
        UserSettings.tiltCalibration = Input.acceleration;
    }

    public void PauseGame()
    {
        
        pauseButton.SetActive(false);
        calibrateButton.SetActive(false);
        timer.SetActive(false);
        statusEffectTracker.gameObject.SetActive(false);
        crystalTracker.gameObject.SetActive(false);
        namePanel.SetActive(false);

        joystick.gameObject.SetActive(false);
        

        pausePanel.SetActive(true);
        
        
        Cursor.visible = true;
        levelState = "Paused";
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        timer.SetActive(true);
        crystalTracker.gameObject.SetActive(true);
        namePanel.SetActive(true);
        statusEffectTracker.gameObject.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        switch (PlayerPrefs.GetString("MovementMode", DefaultSettings.movementMode))
        {
            case "DeviceTilting":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                
                calibrateButton.SetActive(true);
                break;
            case "Joystick":
                Cursor.visible = true;
                pauseButton.SetActive(true);
                
                joystick.gameObject.SetActive(true);
                break;
            case "Keyboard":
                Cursor.visible = false;
                break;
            default:
                break;
        }
        
        levelState = "InGame";
    }

    private void OnApplicationFocus(bool focus)
    {
        //if (!focus && levelState == "InGame")
        //{
        //    PauseGame();
        //}
    }
}


