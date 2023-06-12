using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAction : MonoBehaviour
{
    public Vector3 deltaPosition;
    public Vector3 deltaAngle;

    public float moveTime;
    public float startDelayTime;

    Vector3 startPosition;
    Quaternion startRotation;
    Rigidbody rb;

    bool moved = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = rb.position;
        startRotation = rb.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        if (!moved)
        {
            moved = true;
            StartCoroutine(MoveRoutine());
        }
    }
    public void MoveSine()
    {
        if (!moved)
        {
            moved = true;
            StartCoroutine(MoveSineRoutine());
        }
    }
    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(startDelayTime);
        float t = 0;
        while (t < moveTime)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, startPosition + deltaPosition, t / moveTime));
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(startPosition + deltaPosition);
    }
    IEnumerator MoveSineRoutine()
    {
        yield return new WaitForSeconds(startDelayTime);
        float t = 0;
        Vector3 midPosition = startPosition + deltaPosition / 2;
        while (t < moveTime)
        {
            float sinValue = Mathf.Sin((t / moveTime) * Mathf.PI - 0.5f * Mathf.PI);
            rb.MovePosition(midPosition + (deltaPosition / 2) * sinValue);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(startPosition + deltaPosition);
    }
}
