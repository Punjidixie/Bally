using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeball : Tilting {

    private Vector3 frontVelo;
    private Vector3 sidewayVelo;
    public float speed;

    protected override void TiltByKeyboard()
    {
        TypicalControl();

    }

    protected override void Move()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        rb.MovePosition(rb.position + (sidewayVelo + frontVelo) * Time.fixedDeltaTime);
    }

    private float NormalizeAngle(float angle)
    {
        //Normalize [0,360)
        if (angle >= 0f) { return angle - Mathf.Floor(angle / 360f) * 360; }
        else { return 360f - NormalizeAngle(-angle); }
    }

    private float NormalizeAngle2(float angle)
    {
        //Normalize (-180,180]
        float norm = NormalizeAngle(angle);
        if (norm > 180) { return norm - 360; }
        else return norm;
    }

    private void TiltByKeyboardFree()
    {
        toXRotation = Camera.main.transform.eulerAngles.x;
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;

        toXRotation -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("MouseSensitivity");
        toYRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");

        toXRotation = NormalizeAngle2(toXRotation);
        toXRotation = Mathf.Clamp(toXRotation, xRotationOffset - frontTiltAngle, xRotationOffset + frontTiltAngle);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize;

        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * -forceSize;
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

    private void TiltByMouse()
    {
        toXRotation = Camera.main.transform.eulerAngles.x;
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Camera.main.transform.eulerAngles.z;

        frontForce = Vector3.zero;
        sidewayForce = Vector3.zero;

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.S))
        {
            toXRotation -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("MouseSensitivity");
            toZRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");
        }

        else
        {
            toXRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, xRotationOffset, Time.deltaTime / 0.1f);
            toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

            toYRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");
        }

        toXRotation = Mathf.Clamp(NormalizeAngle2(toXRotation), xRotationOffset - frontTiltAngle, xRotationOffset + frontTiltAngle);
        toZRotation = Mathf.Clamp(NormalizeAngle2(toZRotation), -sideTiltAngle, sideTiltAngle);

        float frontTiltness = -(toXRotation - xRotationOffset) / frontTiltAngle;
        float sideTiltness = toZRotation / sideTiltAngle;


        frontForce = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * forceSize * frontTiltness;

        sidewayForce = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * forceSize * sideTiltness;
    }

    private void TypicalControl()
    {
        toXRotation = Camera.main.transform.eulerAngles.x;
        toYRotation = Camera.main.transform.eulerAngles.y;
        toZRotation = Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, 0, Time.deltaTime / 0.1f);

        frontVelo = Vector3.zero;
        sidewayVelo = Vector3.zero;

        toXRotation -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("MouseSensitivity");
        toYRotation += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("MouseSensitivity");

        toXRotation = NormalizeAngle2(toXRotation);
        toXRotation = Mathf.Clamp(toXRotation, xRotationOffset - frontTiltAngle, xRotationOffset + frontTiltAngle);


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            frontVelo = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * speed;

        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            frontVelo = new Vector3(Mathf.Sin(toYRotation * Mathf.Deg2Rad), 0, Mathf.Cos(toYRotation * Mathf.Deg2Rad)) * -speed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            sidewayVelo = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * -speed;

        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            sidewayVelo = new Vector3(Mathf.Cos(toYRotation * Mathf.Deg2Rad), 0, -Mathf.Sin(toYRotation * Mathf.Deg2Rad)) * speed;

        }
    }
}
