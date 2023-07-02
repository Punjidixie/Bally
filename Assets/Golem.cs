using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{

    public GameObject boulder;
    public float boulderSpeed;

    public GameObject orb;
    public float orbSpeed;

    public GameObject beam;

    // as locations to shoot projectiles from
    public GameObject rightHand;
    public GameObject leftHand;

    // the beams spawn here
    public GameObject rightEye;
    public GameObject leftEye;

    Tilting theBall;
    Rigidbody rb;

    float boulderBarrageWidth = 11f;
    float orbBarrageWidth = 16f;
    float spawnWidth = 20f;
    float spawnFrontOffset = 8f;

    // If it should look at the player
    bool looking = true;

    // degrees per second
    public float turnSpeed;

    // unturned angle
    float defaultAngle;

    float angleBeforeBeaming;

    List<GameObject> orbList = new List<GameObject>();

    enum GolemState
    {
        Phase1,
        Phase2,
        Phase3
    }

    // Start is called before the first frame update
    void Start()
    {
        theBall = FindObjectOfType<Tilting>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            looking = !looking;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(MoveSineRoutine(rb.position + Vector3.left * 30f, 10f));
        }
    }

    void FixedUpdate()
    {
        Vector3 targetVec;

        if (looking)
        {
            targetVec = theBall.transform.position - rb.position;
        }
        else
        {
            targetVec = GetDefaultFront();
        }

        targetVec.y = 0;

        float angleDifference = Vector3.Angle(GetFront(), targetVec);

        float direction = (Vector3.Cross(GetFront(), targetVec).y > 0) ? 1f : -1f;

        float change = direction * Time.fixedDeltaTime * turnSpeed;

        if (Mathf.Abs(change) > angleDifference)
        {
            change = direction * angleDifference;
        }

        rb.MoveRotation(Quaternion.Euler(0f, rb.rotation.eulerAngles.y + change, 0f));

    }


    // Right shift
    Vector3 RotateShiftVector(Vector3 vector, float shift)
    {
        float r = new Vector2(vector.x, vector.z).magnitude;

        float kxr = shift * vector.x / r;
        float kzr = shift * vector.z / r;

        return new Vector3(vector.x + kzr, vector.y, vector.z - kxr);
    }

    public void ShootBoulderRight() { ShootBoulderAt(rightHand.transform.position, theBall.transform.position, 0.75f, 0.5f); }

    public void ShootBoulderLeft() { ShootBoulderAt(leftHand.transform.position, theBall.transform.position, -0.75f, 0.5f); }

    public void ShootDoubleBoulders()
    {
        ShootBoulderLeft();
        ShootBoulderRight();
    }


    // These two are used in animation events
    public void ShootBoulderLeftFront(float shiftIndex) { ShootBoulderAt(leftHand.transform.position, GetBallProjectedPosition(), IndexToShift(shiftIndex, boulderBarrageWidth), 0.5f); }
    public void ShootBoulderRightFront(float shiftIndex) { ShootBoulderAt(rightHand.transform.position, GetBallProjectedPosition(), IndexToShift(shiftIndex, boulderBarrageWidth), 0.5f); }


    void ShootBoulderAt(Vector3 from, Vector3 to, float rightShift, float upShift)
    {
        Vector3 direction = RotateShiftVector(to - from + Vector3.up * upShift, rightShift);

        GameObject boulder1 = Instantiate(boulder);
        Rigidbody rb1 = boulder1.GetComponent<Rigidbody>();
        rb1.position = from;
        rb1.velocity = direction.normalized * boulderSpeed;
    }

    void SpawnOrb(Vector3 position)
    {
        GameObject spawnedOrb = Instantiate(orb);
        spawnedOrb.transform.position = position;

        orbList.Add(spawnedOrb);
    }

    void ShootOrb(GameObject spawnedOrb, Vector3 direction)
    {
        Rigidbody rb = spawnedOrb.GetComponent<Rigidbody>();

        rb.velocity = direction.normalized * orbSpeed;
    }

    public void SpawnHandOrbs()
    {
        SpawnOrb(leftHand.transform.position);
        SpawnOrb(rightHand.transform.position);
    }

    public void ShootHandOrbs()
    {
        ShootOrb(orbList[0], RotateShiftVector(theBall.transform.position - orbList[0].transform.position, -1.5f));
        ShootOrb(orbList[1], RotateShiftVector(theBall.transform.position - orbList[1].transform.position, 1.5f));
        orbList.Clear();
    }

    public void SpawnOrbBarrage()
    {
        // Starts -2, ends 2, total of 4 orbs
        for (float f = -2f; f < 3; f += 4f/3f)
        {
            float frontOffset = (spawnFrontOffset - Mathf.Abs(f) * 2);

            Vector3 spawnDirection = RotateShiftVector(GetFront() * frontOffset, f * (spawnWidth / 4));

            SpawnOrb(transform.position + spawnDirection + Vector3.up * 2);

            Debug.Log(f);
        }
    }

    public void ShootOrbBarrage()
    {
        int i = 0;
        for (float f = -2f; f < 3; f += 4f/3f)
        {
            Vector3 direction = RotateShiftVector(GetBallProjectedPosition() - orbList[i].transform.position, IndexToShift(f, orbBarrageWidth));
            ShootOrb(orbList[i], direction);
            i++;
        }
        orbList.Clear();
    }

    public void SpawnBeams()
    {
        looking = false;
        angleBeforeBeaming = defaultAngle;
        defaultAngle = GolemAngleToLook(theBall.transform.position - transform.position);

        Vector3 rightBeamDirection = RotateShiftVector(theBall.transform.position - rightEye.transform.position, 1f);
        Vector3 leftBeamDirection = RotateShiftVector(theBall.transform.position - leftEye.transform.position, -1f);

        GameObject rightBeam = Instantiate(beam);
        rightBeam.transform.position = rightEye.transform.position;
        rightBeam.transform.rotation = Quaternion.FromToRotation(Vector3.back, rightBeamDirection);


        GameObject leftBeam = Instantiate(beam);
        leftBeam.transform.position = leftEye.transform.position;
        leftBeam.transform.rotation = Quaternion.FromToRotation(Vector3.back, leftBeamDirection);

    }

    public void RecoverFromBeaming()
    {
        looking = true;
        defaultAngle = angleBeforeBeaming;
    }

    // -2 to 2
    float IndexToShift(float i, float width)
    {
        return i * (width / 4);
    }


    // Get the direction the golem is looking at
    Vector3 GetFront() {

        return new Vector3(-Mathf.Sin(rb.rotation.eulerAngles.y * Mathf.Deg2Rad), 0f, -Mathf.Cos(rb.rotation.eulerAngles.y * Mathf.Deg2Rad)).normalized;

    }

    // Get the direction the golem is looking at
    Vector3 GetDefaultFront()
    {

        return new Vector3(-Mathf.Sin(defaultAngle * Mathf.Deg2Rad), 0f, -Mathf.Cos(defaultAngle * Mathf.Deg2Rad)).normalized;

    }

    // Returns the position where the ball would be if it were to be projected in front of the golem
    Vector3 GetBallProjectedPosition()
    {
        Vector3 front = GetFront();

        Vector3 result = transform.position + Projection(theBall.transform.position - transform.position, front);

        return new Vector3(result.x, theBall.transform.position.y, result.z);
    }


    Vector3 Projection(Vector3 point, Vector3 direction) {

        return Vector3.Dot(point, direction.normalized) * direction.normalized;

    }

    IEnumerator MoveSineRoutine(Vector3 to, float moveTime)
    {
        looking = false;
        float t = 0;
        Vector3 startPosition = rb.position;
        Vector3 deltaPosition = to - startPosition;
        Vector3 midPosition = startPosition + deltaPosition / 2;

        defaultAngle = GolemAngleToLook(deltaPosition);

        while (t < moveTime)
        {
            float sinValue = Mathf.Sin((t / moveTime) * Mathf.PI - 0.5f * Mathf.PI);
            rb.MovePosition(midPosition + (deltaPosition / 2) * sinValue);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(startPosition + deltaPosition);
    }

    // Find the golem's looking angle, from a direction vector
    float GolemAngleToLook(Vector3 to)
    {
        Vector3 from = Vector3.back;
        to.y = 0;

        float angle = Vector3.Angle(from, to);

        // Unity's Cross uses left hand rule... ok that clears the earlier confusion up
        return Vector3.Cross(from, to).y > 0 ? angle : -angle;
    }

    // These three functions below were scrapped, but might be useful in the future?
    //Vector3 GetShootDestination(Vector3 from)
    //{
    //    return from + GetScaledFront() - Vector3.up * (from.y - theBall.transform.position.y);
    //}

    //// Front, scaled to the where the ball is projected to
    //Vector3 GetScaledFront()
    //{
    //    float rxz = Projection(theBall.transform.position - transform.position, GetFront()).magnitude;

    //    return GetFront() * rxz;
    //}

    //Vector3 RotateAngleVector(Vector3 vector, float angle)
    //{
    //    float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
    //    float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

    //    return new Vector3(vector.x * cos - vector.z * sin, vector.y, vector.x * sin + vector.z * cos);
    //}



}
