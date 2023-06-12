using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tilting : MonoBehaviour
{
    protected Rigidbody rb;
    protected Vector3 sidewayForce; //direction to the RIGHT
    protected Vector3 frontForce; //direction to the FRONT


    public float forceSize;

    //input
    protected FixedTouchField scrollArea;
    protected Joystick joystick;

    //magnetic field
    bool magnetActive;
    public GameObject magneticField;
    float magnetTime;
    float magnetMaxTime;

    //bouncy effect
    bool bouncyActive;
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
    protected float toXRotation;
    protected float toYRotation;
    protected float toZRotation;

    public float frontTiltAngle;
    public float sideTiltAngle;

    public float heightToCamera; //B'
    public float lengthToCamera; //C'
    public float xRotationOffset;

    [HideInInspector]
    public LevelController levelController;

    //other components
    protected SphereCollider sphereCollider;

    private int frame;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        levelController = FindObjectOfType<LevelController>();

        magnetActive = false;
        bouncyActive = false;
        physicsMaterial.bounciness = 0.7f;

        scrollArea = levelController.scrollArea;
        joystick = levelController.joystick;

        toXRotation = xRotationOffset;
        toYRotation = 0;
        toZRotation = 0;

    }

    private void FixedUpdate()
    {
        if (levelController.levelState == "InGame")
        {
            Move(); // Apply force
        }

    }

    protected virtual void Move()
    {
        rb.AddForce(frontForce, ForceMode.Acceleration);
        rb.AddForce(sidewayForce, ForceMode.Acceleration);
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
                switch (PlayerPrefs.GetString("MovementMode"))
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
                break;
            case "Winning":
                //PanWinning();
                break;
            case "Losing":
                //PanWinning();
                break;
        }


    }

    private void LateUpdate()
    {
        switch (levelController.levelState)
        {
            case "InGame":
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

    protected virtual void TiltByKeyboard()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, xRotationOffset, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;

        toYRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize;

            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, xRotationOffset - frontTiltAngle, Time.deltaTime / 0.1f);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * -forceSize;

            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, xRotationOffset + frontTiltAngle, Time.deltaTime / 0.1f);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * -forceSize;

            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, -sideTiltAngle, Time.deltaTime / 0.1f);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize;

            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, sideTiltAngle, Time.deltaTime / 0.1f);
        }

    }

    protected void TiltByJoystick()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, xRotationOffset, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y; //will be set via levelController
        
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;



        toYRotation = Camera.main.transform.eulerAngles.y + scrollArea.TouchDist.x * PlayerPrefs.GetFloat("DragSensitivity");


        if (joystick.Vertical != 0)
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize * joystick.Vertical;

            toXRotation = xRotationOffset - frontTiltAngle * joystick.Vertical;
        }

        if (joystick.Horizontal != 0)
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * joystick.Horizontal;

            toZRotation = sideTiltAngle * joystick.Horizontal;

        }
    }

    protected void TiltByDeviceTilting()
    {
        toXRotation = xRotationOffset;
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = 0;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;



        toYRotation = Camera.main.transform.eulerAngles.y + scrollArea.TouchDist.x * PlayerPrefs.GetFloat("DragSensitivity");

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

    float ChaseParabola(float x)
    {
        return -4 * Mathf.Pow(x - 0.5f, 2) + 1;
    }

    void ChaseCamera()
    {
        float chasePerVelocity = 15;

        // find target angle (y)
        float yRotationTarget = Vector3.Angle(Vector3.forward, new Vector3(rb.velocity.x, 0, rb.velocity.z));
        if (Vector3.Cross(Vector3.forward, new Vector3(rb.velocity.x, 0, rb.velocity.z)).y < 0)
        {
            yRotationTarget = 360 - yRotationTarget;
        }

        Vector3 targetVec = new Vector3(Mathf.Sin(yRotationTarget * Mathf.Deg2Rad), 0, Mathf.Cos(yRotationTarget * Mathf.Deg2Rad));
        Vector3 currentVec = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad));

        float chaseSpeedFactor = ChaseParabola(Vector3.Angle(targetVec, currentVec) / 181);
        float chaseSpeed = chaseSpeedFactor * chasePerVelocity * Mathf.Pow(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude, 2);

        float direction = 1;

        if (Vector3.Cross(currentVec, targetVec).y < 0)
        {
            direction = -1;
        }

        float change = direction * Time.deltaTime * chaseSpeed;

        if (Mathf.Abs(change) > Vector3.Angle(targetVec, currentVec))
        {
            toYRotation = yRotationTarget;
        }
        else
        {
            toYRotation += change;
        }
    }


    public void UpdateCamera()
    {

        if (PlayerPrefs.GetString("CameraMode") == "Auto")
        {
            ChaseCamera();
        } 

        float xRad = toXRotation * Mathf.Deg2Rad;
        float yRad = toYRotation * Mathf.Deg2Rad;
        float zRad = toZRotation * Mathf.Deg2Rad;

        float h = lengthToCamera * Mathf.Cos(xRad) - heightToCamera * Mathf.Sin(xRad);
        float v = heightToCamera * Mathf.Cos(xRad) + lengthToCamera * Mathf.Sin(xRad);
        float mr = heightToCamera * (1 - Mathf.Cos(zRad)) * Mathf.Sin(xRad);


        float toXPosition = transform.position.x - h * Mathf.Sin(yRad) - heightToCamera * Mathf.Sin(zRad) * Mathf.Cos(yRad) - mr * Mathf.Sin(yRad);
        float toZPosition = transform.position.z - h * Mathf.Cos(yRad) + heightToCamera * Mathf.Sin(zRad) * Mathf.Sin(yRad) - mr * Mathf.Cos(yRad);
        float toYPosition = transform.position.y + v - heightToCamera * (1 - Mathf.Cos(zRad)) * Mathf.Cos(xRad);


        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
        Camera.main.transform.rotation = Quaternion.Euler(toXRotation, toYRotation, toZRotation);
    }

    void PanWinning()
    {
        UpdateCamera();
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

        bool hardHit = Mathf.Abs(Vector3.Dot(collision.impulse.normalized, collision.relativeVelocity)) > bounceSoundThreshold; // dot to prevent the glitch where the flat ground sometimes trigger collision

        //This if statement is for sound playing
        if (hardHit)
        {
            GameObject impulseParticleInstance = Instantiate(impulseParticle);
            impulseParticleInstance.transform.position = collision.GetContact(0).point + collision.impulse.normalized * 0.15f;
            impulseParticleInstance.transform.rotation *= Quaternion.FromToRotation(new Vector3(0, 1, 0), collision.impulse);
            Destroy(impulseParticleInstance, 5);
        }

        if (CheckBreakableCollision(collision))
        {
            // Don't play sound
        }
        else if (hardHit)
        {
            GameObject bounceSoundInstance = Instantiate(bounceSound);
            Destroy(bounceSoundInstance, 5);
        }

        SetAngularVelocity(collision);

    }

    //Returns sound GameObject
    public bool CheckBreakableCollision(Collision collision)
    {
        Breakable b = collision.gameObject.GetComponent<Breakable>();

        if (b) { return b.WillCrack(collision); }
        else { return false; }
        
    }

    public void OnCollisionStay(Collision collision)
    {
        SetAngularVelocity(collision);

    }

    public void SetAngularVelocity(Collision collision)
    {

        Vector3 normal = rb.position - collision.GetContact(0).point;
        normal.Normalize();

        Vector3 relativeVelocity = collision.relativeVelocity;

        //See if the other body rotates
        if (collision.rigidbody)
        {
            relativeVelocity += GetPointVelocity(collision.GetContact(0).point, collision.rigidbody);
        }

        float smallNumber = Time.deltaTime;
        
        //FOR BALL ROTATION
        Vector3 axis = Vector3.Cross(normal, relativeVelocity);
        float angVelMagnitude = axis.magnitude / sphereCollider.radius;

        //Use 1 frame then magnify later so it doesn't go over 360 (or 180 too idk) in quaternion but can get over that later.
        //This will range from 0 to max rotational speed * Time.fixedDeltaTime radians (probably not big enough to go over 180 or 360.
        Vector3 oneFrameRotation = Quaternion.AngleAxis(-angVelMagnitude * smallNumber * Mathf.Rad2Deg, axis).eulerAngles;

        //Negative angles are observed to be added by 360
        if (oneFrameRotation.x > 180) { oneFrameRotation.x -= 360; }
        if (oneFrameRotation.y > 180) { oneFrameRotation.y -= 360; }
        if (oneFrameRotation.z > 180) { oneFrameRotation.z -= 360; }

        rb.angularVelocity = Mathf.Deg2Rad * oneFrameRotation / smallNumber;
    }

    public Vector3 GetPointVelocity(Vector3 point, Rigidbody otherRb)
    {
        //Refer to Rotating Point documentation
        Rigidbody rb = GetComponent<Rigidbody>();

        if (Mathf.Abs(otherRb.angularVelocity.y) > 0)
        {
            float deltaX = point.x - otherRb.position.x;
            float deltaZ = point.z - otherRb.position.z;
            float r = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            Quaternion q = Quaternion.AngleAxis(90, Vector3.up);

            //Rotate by 90 degrees
            return q * new Vector3(deltaX, 0, deltaZ).normalized * otherRb.angularVelocity.y * r; //Note: q must be first (point * q wouldn't compile)
        }
        return Vector3.zero;

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
        physicsMaterial.bounciness = 0.99f;
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