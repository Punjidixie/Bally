using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostModeController : MonoBehaviour
{
    [SerializeField]
    private float defaultMovementSpeed;

    [SerializeField]
    private float defaultRotationSpeed;

    private float movementSpeed;
    private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = defaultMovementSpeed;
        rotationSpeed = defaultRotationSpeed;
    }

    public void UpdateGhostMode()
    {
        UpdateGhostSettings();
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateGhostSettings()
    {

    }

    void UpdateCameraPosition()
    {
        Camera cam = Camera.main;
        Vector3 camRotation = cam.transform.rotation.eulerAngles;

        float frontInput = 0;
        float sideInput = 0;

        if (Input.GetKey(KeyCode.W)) { frontInput = 1; }
        else if (Input.GetKey(KeyCode.S)) { frontInput = -1; }

        if (Input.GetKey(KeyCode.A)) { sideInput = -1; }
        else if (Input.GetKey(KeyCode.D)) { sideInput = 1; }


        // calculating the direction vectors
        // between 0 and 1
        float xzPlaneMagnitude = Mathf.Abs(defaultMovementSpeed * Mathf.Cos(camRotation.x * Mathf.Deg2Rad));

        // front will move
        Vector3 frontVelocity = new Vector3(
            Mathf.Sin(camRotation.y * Mathf.Deg2Rad) * xzPlaneMagnitude,
            -Mathf.Sin(camRotation.x * Mathf.Deg2Rad),
            Mathf.Cos(camRotation.y * Mathf.Deg2Rad) * xzPlaneMagnitude
            ) * frontInput;

        Vector3 sidewayVelocity = new Vector3(
            Mathf.Cos(camRotation.y * Mathf.Deg2Rad) * xzPlaneMagnitude,
            0,
            -Mathf.Sin(camRotation.y * Mathf.Deg2Rad) * xzPlaneMagnitude
            ) * sideInput;

        cam.transform.position += (frontVelocity + sidewayVelocity) * movementSpeed * Time.deltaTime;
    }

    void UpdateCameraRotation()
    {
        Vector3 camRotation = Camera.main.transform.rotation.eulerAngles;


        // clean up NaN because Input.GetAxis returns NaN values on frames that Update() was never called e.g. the unpausing frame
        // divide by deltatime because of the way mouse sensitivity is calibrated
        float xInput = CleanUpNaN(-Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("MouseSensitivity") / Time.deltaTime);
        float yInput = CleanUpNaN(Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity") / Time.deltaTime);


        if (Input.GetKey(KeyCode.UpArrow)) { xInput = -rotationSpeed; }
        else if (Input.GetKey(KeyCode.DownArrow)) { xInput = rotationSpeed; }

        if (Input.GetKey(KeyCode.LeftArrow)) { yInput = -rotationSpeed; }
        else if (Input.GetKey(KeyCode.RightArrow)) { yInput = rotationSpeed; }


        Camera.main.transform.rotation = Quaternion.Euler(
            camRotation.x + xInput * Time.deltaTime,
            camRotation.y + yInput * Time.deltaTime,
            camRotation.z);
    }

    float CleanUpNaN(float num)
    {
        if (float.IsNaN(num)) { return 0f; }
        else { return num; }

    }
}
