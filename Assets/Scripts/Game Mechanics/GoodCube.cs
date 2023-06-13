using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCube : Triggerable
{

    public GameObject winLight;
    public GameObject cube;

    public bool active;
    float amplitude;
    float period;
    float angularVelocity;
    float timePassed;
    Vector3 startingPosition;

    LevelController levelController;

    // Inherit TriggerType
    public GoodCube() { triggerType = TriggerType.WinBox; }

    // Start is called before the first frame update
    void Start()
    {
        amplitude = 0.25f;
        period = 1.5f;
        startingPosition = cube.transform.position;
        angularVelocity = 180;
        timePassed = 0;
        active = false;

        levelController = FindObjectOfType<LevelController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //winLight.transform.position = new Vector3(winLight.transform.position.x, winLightStartingPosition.y, winLight.transform.position.z);

        float speedFactor;
        if (levelController.reqCrystal1 == 0) { speedFactor = 1; }
        else { speedFactor = Mathf.Clamp(levelController.crystals / levelController.reqCrystal1, 0, 1); }

        timePassed += Time.deltaTime * speedFactor; // For position up and down only, since rotation is constant and doesn't use trigonometry.

        cube.transform.position = startingPosition + new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * (timePassed / period)), 0);
        cube.transform.rotation = Quaternion.Euler(cube.transform.eulerAngles.x, cube.transform.eulerAngles.y + angularVelocity * Time.deltaTime * speedFactor , cube.transform.eulerAngles.z);
        if (active)
        {
            winLight.SetActive(true);
        }
    }

    

    

    
}
