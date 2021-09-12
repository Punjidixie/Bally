using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int value;

    float amplitude;
    float period;
    float angularVelocity;
    float timePassed;
    Vector3 startingPosition;

    //debug
    float maxY;
    float minY;
    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0.15f;
        period = 1.5f;
        startingPosition = transform.position;
        angularVelocity = 180f;
        timePassed = 0f;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        timePassed += Time.fixedDeltaTime;

        transform.position += new Vector3(0f, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), 0f) * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.fixedDeltaTime, transform.eulerAngles.z);


    }
    void Update()
    {
        
    }

    void OldUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        timePassed += Time.deltaTime;

        //transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
        //transform.position += new Vector3(0, amplitude * Mathf.Sin((timePassed / period) * Mathf.PI * 2), 0);
        //rb.velocity = new Vector3(rb.velocity.x, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), rb.velocity.z);
        transform.position += new Vector3(0f, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), 0f) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.deltaTime, transform.eulerAngles.z);

        if (transform.position.y >= maxY)
        {
            maxY = transform.position.y;
        }
        if (transform.position.y <= minY)
        {
            minY = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(maxY - startingPosition.y);
        }
        //Debug.Log(startingPosition.y - transform.position.y);
    }






}
