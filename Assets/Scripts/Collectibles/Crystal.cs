using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Triggerable
{

    public int value;

    public GameObject collectParticles;

    public GameObject collectSound;

    float angularVelocity;

    // Inherit TriggerType
    public Crystal() { triggerType = TriggerType.Crystal; }

    //float amplitude;
    //float period;
    //float timePassed;
    //Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        angularVelocity = 180f;
        //amplitude = 0.5f;
        //period = 3;
        //startingPosition = transform.position;
        //timePassed = 0f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ////Commented out horizontal jiggling
        //timePassed += Time.fixedDeltaTime;
        ////delta position = deriverative of position *delta time
        //transform.position += new Vector3(0f, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), 0f) * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.fixedDeltaTime, transform.eulerAngles.z);


    }

    public void GetCollected()
    {
        //Spawn particles
        GameObject particle = Instantiate(collectParticles, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(particle, 5);

        //Play sound
        GameObject sound = Instantiate(collectSound);
        Destroy(sound, 5);

        //Cease to exist :(
        Destroy(gameObject);

    }

    //void OldUpdate()
    //{
    //    Rigidbody rb = GetComponent<Rigidbody>();
    //    timePassed += Time.deltaTime;

    //    //transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
    //    //transform.position += new Vector3(0, amplitude * Mathf.Sin((timePassed / period) * Mathf.PI * 2), 0);
    //    //rb.velocity = new Vector3(rb.velocity.x, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), rb.velocity.z);
    //    transform.position += new Vector3(0f, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), 0f) * Time.deltaTime;
    //    transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.deltaTime, transform.eulerAngles.z);

    //    if (transform.position.y >= maxY)
    //    {
    //        maxY = transform.position.y;
    //    }
    //    if (transform.position.y <= minY)
    //    {
    //        minY = transform.position.y;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        Debug.Log(maxY - startingPosition.y);
    //    }
    //    //Debug.Log(startingPosition.y - transform.position.y);
    //}






}
