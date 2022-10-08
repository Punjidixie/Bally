using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetaSelTilting : MonoBehaviour
{

    Rigidbody rb;
    Vector3 sidewayForce; //direction to the RIGHT
    Vector3 frontForce; //direction to the FRONT

    float forceSize;

    float toXRotation;
    float toYRotation;
    float toZRotation;

    float toXPosition;
    float toYPosition;
    float toZPosition;

    float frontTiltAngle;
    float sideTiltAngle;

    public static float distanceToCameraNormal;
    float distanceToCameraMax;
    float distanceToCameraMin;
    float distanceToCamera;

    float mouseSensitivity;

    public BetaSelController bsc;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        forceSize = 8;

        frontTiltAngle = 15;
        sideTiltAngle = 20;

        distanceToCameraNormal = 7f;
        distanceToCameraMax = 7.5f;
        distanceToCameraMin = 6.5f;
        distanceToCamera = distanceToCameraNormal;

        mouseSensitivity = 10;
    }
    private void FixedUpdate()
    {
        rb.AddForce(frontForce, ForceMode.Acceleration);
        rb.AddForce(sidewayForce, ForceMode.Acceleration);
    }
    // Update is called once per frame
    void Update()
    {
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

        toYRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");

        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    bsc.PauseGame();
        //}


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

        float distanceToCameraH = distanceToCamera * Mathf.Cos(toXRotation * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(toYRotation * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(toYRotation * Mathf.Deg2Rad);
        toYPosition = transform.position.y + 3f + distanceToCamera * Mathf.Sin(toXRotation * Mathf.Deg2Rad);

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

        //Debug.Log((Camera.main.transform.position - transform.position - new Vector3(0, 1, 0)).magnitude);

    }

    void TiltByJoystick()
    {
        toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, 0, Time.deltaTime / 0.1f);
        toYRotation = Camera.main.transform.eulerAngles.y; //will be set via bsc

        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        toXPosition = 0;
        toYPosition = transform.position.y + 1;
        toZPosition = 0;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;


        toYRotation = Camera.main.transform.eulerAngles.y + bsc.scrollArea.TouchDist.x * PlayerPrefs.GetFloat("DragSensitivity");

        if (bsc.joystick.Vertical != 0)
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize * bsc.joystick.Vertical;

            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, -frontTiltAngle * bsc.joystick.Vertical, Time.deltaTime / 0.1f);
        }

        if (bsc.joystick.Horizontal != 0)
        {
            sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * bsc.joystick.Horizontal;

            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, sideTiltAngle * bsc.joystick.Horizontal, Time.deltaTime / 0.1f);
        }



        float distanceToCameraH = distanceToCamera * Mathf.Cos(toXRotation * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(toYRotation * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(toYRotation * Mathf.Deg2Rad);
        toYPosition = transform.position.y + 3f + distanceToCamera * Mathf.Sin(toXRotation * Mathf.Deg2Rad);

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



        //experimental chase camera
        //if (rb.velocity.x != 0 && rb.velocity.z != 0)
        //{
        //    toYRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg, Time.deltaTime / 0.5f);
        //}

        toYRotation = Camera.main.transform.eulerAngles.y + bsc.scrollArea.TouchDist.x * PlayerPrefs.GetFloat("DragSensitivity");

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




        float distanceToCameraH = distanceToCamera * Mathf.Cos(toXRotation * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(toYRotation * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(toYRotation * Mathf.Deg2Rad);
        toYPosition = transform.position.y + 3f + distanceToCamera * Mathf.Sin(toXRotation * Mathf.Deg2Rad);

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

        //Debug.Log((Camera.main.transform.position - transform.position - new Vector3(0, 1, 0)).magnitude);

    }


    void PanWinning()
    {
        float distanceToCameraH = distanceToCamera * Mathf.Cos(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        toXPosition = transform.position.x - distanceToCameraH * Mathf.Sin(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toZPosition = transform.position.z - distanceToCameraH * Mathf.Cos(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad);
        toYPosition = transform.position.y + 1.5f + distanceToCamera * Mathf.Sin(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);

        Camera.main.transform.position = new Vector3(toXPosition, toYPosition, toZPosition);
    }
}
