using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustRotate : MonoBehaviour
{
    public bool rotateX, rotateY, rotateZ = false;
    public bool clockwise;
    public float period;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

        transform.rotation = Quaternion.Euler(toXRotation, toYRotation, toZRotation);
    }


}
