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

    //crystal sound
    public GameObject crystalSound;

    //bouncy sound
    public GameObject bounceSound;
    public float bounceSoundThreshold;
    

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

    float mouseSensitivity;
    public LevelController levelController;

  
    // Start is called before the first frame update
    void Start()
    {
        magnetActive = false;
        bouncyActive = false;
        physicsMaterial.bounciness = 0.7f;
        forceSize = 8;
        //OpenMagneticField();
        rb = GetComponent<Rigidbody>();
        //forceSize = 8;
        
        frontTiltAngle = 15;
        sideTiltAngle = 20;


        heightToCamera = 1f;
        xRotationOffset = 20f;

        distanceToCameraNormal = 3.75f;
        distanceToCameraMax = distanceToCameraNormal + 0.5f;
        distanceToCameraMin = distanceToCameraNormal - 0.5f;
        distanceToCamera = distanceToCameraNormal;

        mouseSensitivity = 10;

        //StartCoroutine(CheckForBounce());
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
        
        if (levelController.levelState == "Intro" || levelController.levelState == "Panning")
        {
            rb.useGravity = false;
        }
        else if (levelController.levelState == "InGame")
        {
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    levelController.PauseGame();
            //}
            rb.useGravity = true;
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

            //UpdateCamera();

        }
        
        else if (levelController.levelState == "Paused")
        {
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    levelController.ContinueGame();
            //}
        }

        if (levelController.levelState == "InGame")
        {
            UpdateCamera();
        }
        else if (levelController.levelState == "Winning")
        {
            PanWinning();
        }
        else if (levelController.levelState == "Losing")
        {
            PanWinning();
        }

        

    }

    private void LateUpdate()
    {

        
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


        //ChaseCamera();


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

       

        //Debug.Log((Camera.main.transform.position - transform.position - new Vector3(0, 1, 0)).magnitude);

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
            //toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, -frontTiltAngle * levelController.joystick.Vertical, Time.deltaTime / 0.1f);
        }

        if (levelController.joystick.Horizontal != 0)
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * levelController.joystick.Horizontal;

            toZRotation = sideTiltAngle * levelController.joystick.Horizontal;
            //toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, sideTiltAngle * levelController.joystick.Horizontal, Time.deltaTime / 0.1f);
        }

        //Debug.Log(levelController.joystick.Horizontal);
        

        

        //Debug.Log((Camera.main.transform.position - transform.position - new Vector3(0, 1, 0)).magnitude);

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

        //limit them from 1 to -1
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

        


        

        //Debug.Log((Camera.main.transform.position - transform.position - new Vector3(0, 1, 0)).magnitude);

    }

    void UpdateCamera()
    {
        
        float distanceToCameraH = distanceToCamera * Mathf.Cos(toXRotation * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(toYRotation * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(toYRotation * Mathf.Deg2Rad);
        toYPosition = transform.position.y + heightToCamera + distanceToCamera * Mathf.Sin(toXRotation * Mathf.Deg2Rad);

        Vector3 playerFromCamera = transform.position - new Vector3(toXPosition, toYPosition, toZPosition);
        float relativeFrontSpeed = Vector3.Dot(new Vector2(playerFromCamera.x, playerFromCamera.z), new Vector2(rb.velocity.x, rb.velocity.z));
        distanceToCamera = Mathf.Lerp(distanceToCamera, distanceToCameraNormal + relativeFrontSpeed * 0.03f, Time.deltaTime / 0.5f);

        if (distanceToCamera > distanceToCameraMax)
        {
            distanceToCamera = distanceToCameraMax;
        }
        else if (distanceToCamera < distanceToCameraMin)
        {
            distanceToCamera = distanceToCameraMin;
        }

        
        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
        Camera.main.transform.rotation = Quaternion.Euler(toXRotation, toYRotation, toZRotation);
    }

    void CheckObstruction()
    {
        
    }
    void PanWinning()
    {
        float distanceToCameraH = distanceToCamera * Mathf.Cos(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toYPosition = transform.position.y + heightToCamera + distanceToCamera * Mathf.Sin(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
    }

    void DragPanning()
    {
        toYRotation = Camera.main.transform.eulerAngles.y + levelController.scrollArea.TouchDist.x * UserSettings.dragSensitivity;
        
    }
    

    void ChaseCamera()
    {
        //experimental chase camera
        if (rb.velocity.x != 0 && rb.velocity.z != 0)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg, Camera.main.transform.eulerAngles.y));
            
           
            
            
            toYRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg, Time.deltaTime / 0.25f);
        }
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
            GameObject x = Instantiate(bounceSound);
            Destroy(x, 5);
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