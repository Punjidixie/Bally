using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SkyCamera : MonoBehaviour
{
    LevelController levelController;
    Tilting ball;
    // Start is called before the first frame update
    void Start()
    {
        levelController = FindObjectOfType<LevelController>();
        ball = FindObjectOfType<Tilting>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (levelController)
        {
            switch (levelController.levelState)
            {
                case "InGame":
                case "Paused":
                case "Winning":
                case "Losing":
                case "End":
                    if (ball.isGhostMode) { transform.rotation = Camera.main.transform.rotation; }
                    // If not tilted, the main camera defauts to ball.xRotationOffset.
                    else { transform.rotation = Quaternion.Euler(ball.xRotationOffset, Camera.main.transform.eulerAngles.y, 0); }
                    break;
                default:
                    // Sky camera and main camera should sync up, unless while tilted.
                    transform.rotation = Camera.main.transform.rotation;
                    break;
                
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(ball.xRotationOffset, Camera.main.transform.eulerAngles.y, 0);
        }
        

    }
}
