using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tilting : MonoBehaviour
{
    Rigidbody rb;
    Vector3 sidewayForce; //direction to the RIGHT
    Vector3 frontForce; //direction to the FRONT


    public float forceSize;

    //magnetic field
    public bool magnetActive;
    public GameObject magneticField;
    float magnetTime;
    float magnetMaxTime;

    //bouncy effect
    public bool bouncyActive;
    public GameObject bouncyBubble;
    public PhysicMaterial physicsMaterial;
    float bouncyMaxTime;
    float bouncyTime;

    //particles
    public ParticleSystem coinParticle1;
    public ParticleSystem coinParticle3;
    public GameObject impulseParticle;

    //crystal sound
    public GameObject crystalSound;

    //bouncy sound
    public GameObject bounceSound;
    public float bounceSoundThreshold;

    //switch sound
    public GameObject switchSound;
    

    //for camera
    float toXRotation;
    float toYRotation;
    float toZRotation;

    float toXPosition;
    float toYPosition;
    float toZPosition;

    float frontTiltAngle;
    float sideTiltAngle;

    public float distanceToCameraNormal;
    public float heightToCamera;
    public float xRotationOffset;
    float distanceToCameraMax;
    float distanceToCameraMin;
    float distanceToCamera;

    public LevelController levelController;

  
    // Start is called before the first frame update
    void Start()
    {
        magnetActive = false;
        bouncyActive = false;
        physicsMaterial.bounciness = 0.7f;

        rb = GetComponent<Rigidbody>();

        
        frontTiltAngle = 15;
        sideTiltAngle = 20;


        heightToCamera = 0.8f;
        xRotationOffset = 20f;

        distanceToCameraNormal = 3.75f;
        distanceToCameraMax = distanceToCameraNormal + 0.5f;
        distanceToCameraMin = distanceToCameraNormal - 0.5f;
        distanceToCamera = distanceToCameraNormal;

    }

    private void FixedUpdate()
    {
        if (levelController.levelState == "InGame")
        {
            rb.AddForce(frontForce, ForceMode.Acceleration);
            rb.AddForce(sidewayForce, ForceMode.Acceleration);
        }
        
        
    }
    // Update is called once per frame
    private void Update()
    {

        switch (levelController.levelState)
        {
            case "Intro":
                break;
            case "Panning":
                break;
            case "InGame":
                switch (UserSettings.movementMode)
                {
                    case "Joystick":
                        TiltByJoystick();
                        break;
                    case "Keyboard":
                        TiltByKeyboard();
                        break;
                    case "DeviceTilting":
                        TiltByDeviceTilting();
                        break;
                    default:
                        TiltByKeyboard();
                        break;
                }
                UpdateCamera();
                break;
            case "Winning":
                PanWinning();
                break;
            case "Losing":
                PanWinning();
                break;
        }

    }

    private void LateUpdate()
    {
        //switch (levelController.levelState)
        //{
        //    case "InGame":
        //        UpdateCamera();
        //        break;
        //}

    }
    void TiltByKeyboard()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, 0, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        toXPosition = 0;
        toYPosition = transform.position.y + 1;
        toZPosition = 0;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;

        toYRotation += Input.GetAxis("Mouse X") * UserSettings.mouseSensitivity;


        if (Input.GetKey(KeyCode.K))
        {

            toYRotation = toYRotation - UserSettings.cameraAngularSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.L))
        {

            toYRotation = toYRotation + UserSettings.cameraAngularSpeed * Time.deltaTime;
        }



        if (Input.GetKey(KeyCode.W))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize;

            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, -frontTiltAngle, Time.deltaTime / 0.1f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * -forceSize;

            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, frontTiltAngle, Time.deltaTime / 0.1f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * -forceSize;

            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, -sideTiltAngle, Time.deltaTime / 0.1f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize;

            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, sideTiltAngle, Time.deltaTime / 0.1f);
        }

    }

    void TiltByJoystick()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, 0, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y; //will be set via levelController
        
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        toXPosition = 0;
        toYPosition = transform.position.y + 1;
        toZPosition = 0;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;



        toYRotation = Camera.main.transform.eulerAngles.y + levelController.scrollArea.TouchDist.x * UserSettings.dragSensitivity;


        if (levelController.joystick.Vertical != 0)
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize * levelController.joystick.Vertical;

            toXRotation = -frontTiltAngle * levelController.joystick.Vertical;

        }

        if (levelController.joystick.Horizontal != 0)
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * levelController.joystick.Horizontal;

            toZRotation = sideTiltAngle * levelController.joystick.Horizontal;

        }
    }

    void TiltByDeviceTilting()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, 0, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        toXPosition = 0;
        toYPosition = transform.position.y + 1;
        toZPosition = 0;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;



        toYRotation = Camera.main.transform.eulerAngles.y + levelController.scrollArea.TouchDist.x * UserSettings.dragSensitivity;

        float frontTiltness = -(Input.acceleration.z - UserSettings.tiltCalibration.z) / 0.3f;
        float sideTiltness = (Input.acceleration.x - UserSettings.tiltCalibration.x) / 0.3f;

        if (Mathf.Abs(frontTiltness) > 1)
        {
            frontTiltness /= Mathf.Abs(frontTiltness);
        }
        if (Mathf.Abs(sideTiltness) > 1)
        {
            sideTiltness /= Mathf.Abs(sideTiltness);
        }

        frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize * frontTiltness;

        
        sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * sideTiltness;

       
    }

    public void UpdateCamera()
    {
        
        float distanceToCameraH = distanceToCamera * Mathf.Cos(toXRotation * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(toYRotation * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(toYRotation * Mathf.Deg2Rad);
        toYPosition = transform.position.y + heightToCamera + distanceToCamera * Mathf.Sin(toXRotation * Mathf.Deg2Rad);

        //Vector3 playerFromCamera = transform.position - new Vector3(toXPosition, toYPosition, toZPosition);
        //float relativeFrontSpeed = Vector3.Dot(new Vector2(playerFromCamera.x, playerFromCamera.z), new Vector2(rb.velocity.x, rb.velocity.z));
        //distanceToCamera = Mathf.Lerp(distanceToCamera, distanceToCameraNormal + relativeFrontSpeed * 0.03f, Time.deltaTime / 0.5f);
        
        //if (distanceToCamera > distanceToCameraMax)
        //{
        //    distanceToCamera = distanceToCameraMax;
        //}
        //else if (distanceToCamera < distanceToCameraMin)
        //{
        //    distanceToCamera = distanceToCameraMin;
        //}

        distanceToCamera = distanceToCameraNormal;

        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
        Camera.main.transform.rotation = Quaternion.Euler(toXRotation, toYRotation, toZRotation);
    }

    void PanWinning()
    {
        float distanceToCameraH = distanceToCamera * Mathf.Cos(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toYPosition = transform.position.y + heightToCamera + distanceToCamera * Mathf.Sin(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (levelController.levelState == "InGame")
        {
            switch (other.gameObject.tag)
            {
                case "WinBox":

                    if (other.gameObject.GetComponent<GoodCube>().active)
                    {
                        //Destroy(other.gameObject);

                        rb.AddForce(new Vector3(0, 25, 0), ForceMode.VelocityChange);

                        StartCoroutine(levelController.WinRoutine());
                    }
                    break;

                case "Crystal":

                    levelController.crystalTracker.AddCrystal(other.gameObject.GetComponent<Crystal>().value);
                    levelController.crystalsText.text = "Crystals: " + levelController.crystals.ToString() + "/" + levelController.reqCrystal1.ToString();
                    Destroy(other.gameObject);

                    ParticleSystem particle;
                    switch (other.gameObject.GetComponent<Crystal>().value)
                    {
                        case 1:
                            particle = Instantiate(coinParticle1, other.transform.position, Quaternion.Euler(-90, 0, 0));
                            break;
                        case 3:
                            particle = Instantiate(coinParticle3, other.transform.position, Quaternion.Euler(-90, 0, 0));
                            break;
                        default:
                            particle = Instantiate(coinParticle1, other.transform.position, Quaternion.Euler(-90, 0, 0));
                            break;

                    }
                    
                    Destroy(particle, 5);

                    GameObject sound = Instantiate(crystalSound);
                    Destroy(sound, 5);

                    break;
                case "Switch":
                    Switch switchScript = other.gameObject.GetComponent<Switch>();

                    if (switchScript.canRetrigger)
                    {
                        if (!switchScript.switchOn)
                        {
                            switchScript.TurnOn();
                        }
                        else if (switchScript.switchOn)
                        {
                            switchScript.TurnOff();
                        }
                        GameObject s = Instantiate(switchSound);
                        Destroy(s, 5);
                    }
                    else if (!switchScript.canRetrigger)
                    {
                        if (!switchScript.switchOn)
                        {
                            GameObject s = Instantiate(switchSound);
                            Destroy(s, 5);
                            switchScript.TurnOn();
                        }
                    }
                    
                    break;
                case "Magnet":
                    magnetTime = 0;
                    magnetMaxTime = other.gameObject.GetComponent<Magnet>().maxTime;
                    levelController.statusEffectTracker.magnetSlider.maxValue = magnetMaxTime;
                    levelController.statusEffectTracker.magnetSlider.value = magnetMaxTime;
                    levelController.statusEffectTracker.magnetTracker.SetActive(true);
                    levelController.statusEffectTracker.magnetTracker.transform.SetSiblingIndex(0);

                    if (!magnetActive)
                    {
                        StartCoroutine(WhileMagnetActive()); 
                    }
                    Destroy(other.gameObject);
                    break;
                case "Bouncy":
                    bouncyTime = 0;
                    bouncyMaxTime = other.gameObject.GetComponent<Bouncy>().maxTime;
                    levelController.statusEffectTracker.bouncySlider.maxValue = bouncyMaxTime;
                    levelController.statusEffectTracker.bouncySlider.value = bouncyMaxTime;
                    levelController.statusEffectTracker.bouncyTracker.SetActive(true);
                    levelController.statusEffectTracker.bouncyTracker.transform.SetSiblingIndex(0);

                    if (!bouncyActive)
                    {
                        StartCoroutine(WhileBouncyActive());
                    }
                    Destroy(other.gameObject);
                    break;
                default:
                    break;
            }
        }
        
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (Vector3.Dot(collision.impulse, collision.relativeVelocity) > bounceSoundThreshold)
        {

            GameObject bounceSoundInstance = Instantiate(bounceSound);
            Destroy(bounceSoundInstance, 5);

            GameObject impulseParticleInstance = Instantiate(impulseParticle);
            impulseParticleInstance.transform.position = collision.GetContact(0).point + collision.impulse.normalized * 0.15f;
            impulseParticleInstance.transform.rotation *= Quaternion.FromToRotation(new Vector3(0, 1, 0), collision.impulse);
            Destroy(impulseParticleInstance, 5);
        }
        Debug.Log(Vector3.Dot(collision.impulse, collision.relativeVelocity));
    }
    IEnumerator WhileMagnetActive()
    {
        magnetActive = true;
        magneticField.SetActive(true);
        while (magnetTime < magnetMaxTime)
        {
            magnetTime += Time.deltaTime;
            levelController.statusEffectTracker.magnetSlider.value = magnetMaxTime - magnetTime;
            yield return null;
        }

        levelController.statusEffectTracker.magnetTracker.SetActive(false);
        magnetActive = false;
        magneticField.SetActive(false);
    }

    IEnumerator WhileBouncyActive()
    {
        bouncyActive = true;
        physicsMaterial.bounciness = 0.98f;
        bouncyBubble.SetActive(true);

        while (bouncyTime < bouncyMaxTime)
        {
            bouncyTime += Time.deltaTime;
            levelController.statusEffectTracker.bouncySlider.value = bouncyMaxTime - bouncyTime;
            yield return null;
        }

        levelController.statusEffectTracker.bouncyTracker.SetActive(false);
        bouncyActive = false;
        physicsMaterial.bounciness = 0.7f;
        bouncyBubble.SetActive(false);
    }

    

    
}