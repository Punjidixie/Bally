using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : StatusEffectObject
{
    float amplitude;
    float period;
    float angularVelocity;
    float timePassed;
    Vector3 startingPosition;

    // Inherit TriggerType
    public Bouncy() { statusEffectType = StatusEffectType.Bouncy; }
    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0.15f;
        period = 1.5f;
        startingPosition = transform.position;
        angularVelocity = 180;
        timePassed = 0;

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;

        //transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
        transform.position += new Vector3(0, amplitude * ((2 * Mathf.PI) / period) * Mathf.Cos(2 * Mathf.PI * (timePassed / period)), 0) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.deltaTime, transform.eulerAngles.z);

    }






}