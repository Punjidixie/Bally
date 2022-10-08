using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTilting : Tilting
{

    public SimpleController controller;

    // Start is called before the first frame update
    void Start()
    {
        physicsMaterial.bounciness = 0.7f;

        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        scrollArea = controller.scrollArea;
        joystick = controller.joystick;
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
        UpdateCamera();
    }

    private void FixedUpdate()
    {
        rb.AddForce(frontForce, ForceMode.Acceleration);
        rb.AddForce(sidewayForce, ForceMode.Acceleration);

    }

    
}
