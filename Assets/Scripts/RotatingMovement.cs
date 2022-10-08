using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingMovement : MonoBehaviour
{
    public LevelController levelController;
    public bool rotateX, rotateY, rotateZ = false;
    public bool clockwise;
    public float period;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (levelController.levelState == "InGame")
        {
            float toXRotation = transform.eulerAngles.x;
            float toYRotation = transform.eulerAngles.y;
            float toZRotation = transform.eulerAngles.z;

            if (rotateX && clockwise) { toXRotation += (Time.fixedDeltaTime / period) * 360; }
            if (rotateY && clockwise) { toYRotation += (Time.fixedDeltaTime / period) * 360; }
            if (rotateZ && clockwise) { toZRotation += (Time.fixedDeltaTime / period) * 360; }

            if (rotateX && !clockwise) { toXRotation -= (Time.fixedDeltaTime / period) * 360; }
            if (rotateY && !clockwise) { toYRotation -= (Time.fixedDeltaTime / period) * 360; }
            if (rotateZ && !clockwise) { toZRotation -= (Time.fixedDeltaTime / period) * 360; }

            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(toXRotation, toYRotation, toZRotation));
        }

    }

    
    
}
