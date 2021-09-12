﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMovement : MonoBehaviour
{
    public Vector3 movementScale; //amplitude x 2
    public bool alwaysMoving;
    public float period;
    public float wait;
    public LevelController levelController; //I don't want this either but it's (kinda) necessary
    bool waiting = false;
    bool goingForward = true; //going in the direction of 
    Vector3 startingPosition;
    Vector3 midPosition;
    float timePassed;
    Vector3 force;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        midPosition = startingPosition + movementScale / 2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = GetComponent<Rigidbody>().position;
    }

    private void FixedUpdate()
    {
        if (levelController.levelState == "InGame" || alwaysMoving)
        {
            if (!waiting)
            {
                timePassed += Time.fixedDeltaTime;
                float sinValue = Mathf.Sin((timePassed / period) * 2 * Mathf.PI - 0.5f * Mathf.PI);
                float difSinValue = 2 * (Mathf.PI / period) * Mathf.Cos((timePassed / period) * 2 * Mathf.PI - 0.5f * Mathf.PI);
                if (goingForward && difSinValue < 0)
                {
                    waiting = true;
                    StartCoroutine(Waiting());
                }
                else if (!goingForward && difSinValue > 0)
                {
                    waiting = true;
                    StartCoroutine(Waiting());
                }
                else
                {
                    Vector3 newPos = midPosition + (movementScale / 2) * sinValue;
                    GetComponent<Rigidbody>().MovePosition(newPos);
                }
                
            }
            //force = ((movementScale * 2 * Mathf.PI) / (2 * period)) * Mathf.Cos((timePassed / period) * 2 * Mathf.PI + 1.5f * Mathf.PI);
            //gameObject.GetComponent<Rigidbody>().velocity = force;

        }
    }

    IEnumerator Waiting()
    {
        waiting = true;
        yield return new WaitForSeconds(wait);
        waiting = false;
        goingForward = !goingForward;
    }

    
}
