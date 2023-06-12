using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMovement : MonoBehaviour
{
    public Vector3 movementScale; //amplitude x 2
    public bool alwaysMoving;
    public float period;
    public float wait;
    LevelController levelController;
    bool waiting = false;
    bool goingForward = true; //going in the direction of 
    Vector3 startingPosition;
    Vector3 midPosition;
    float timePassed;
    Vector3 force;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        levelController = FindObjectOfType<LevelController>();
        startingPosition = transform.position;
        midPosition = startingPosition + movementScale / 2;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = rb.position;
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
                Vector3 newPos;
                if (goingForward && difSinValue < 0)
                {
                    newPos = midPosition + (movementScale / 2);
                    waiting = true;
                    StartCoroutine(Waiting());
                }
                else if (!goingForward && difSinValue > 0)
                {
                    newPos = midPosition - (movementScale / 2);
                    waiting = true;
                    StartCoroutine(Waiting());
                }
                else
                {
                    newPos = midPosition + (movementScale / 2) * sinValue;
                }

                newPos = midPosition + (movementScale / 2) * sinValue;
                rb.MovePosition(newPos);

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
