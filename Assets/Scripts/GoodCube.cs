using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCube : MonoBehaviour
{

    public GameObject winLight;

    public bool active;
    float amplitude;
    float period;
    float angularVelocity;
    float timePassed;
    Vector3 startingPosition;
    Vector3 winLightStartingPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0.25f;
        period = 1.5f;
        startingPosition = transform.position;
        winLightStartingPosition = winLight.transform.position;
        angularVelocity = 180;
        timePassed = 0;
        active = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        winLight.transform.position = new Vector3(winLight.transform.position.x, winLightStartingPosition.y, winLight.transform.position.z);
        timePassed += Time.deltaTime;

        transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angularVelocity * Time.deltaTime , transform.eulerAngles.z);
        if (active)
        {
            winLight.SetActive(true);
        }
    }

    

    

    
}
