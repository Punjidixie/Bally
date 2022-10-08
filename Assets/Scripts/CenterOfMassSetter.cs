using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMassSetter : MonoBehaviour
{
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
