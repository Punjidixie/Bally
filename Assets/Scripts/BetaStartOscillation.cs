using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetaStartOscillation : MonoBehaviour
{
    public int value;

    float amplitude;
    float period;
    float angularVelocity;
    float timePassed;
    Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        amplitude = 1f;
        period = 3f;
        startingPosition = transform.position;
        angularVelocity = 180;
        timePassed = 0;

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;

        transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.deltaTime, transform.eulerAngles.z);

    }






}
