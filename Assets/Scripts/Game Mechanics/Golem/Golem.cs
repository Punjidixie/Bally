using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolemState
{
    Phase0,
    Phase1,
    Phase2,
    Phase3,
    Phase4,
    Phase5
}

// This class contains functions telling the golem what to do. These are called in the animation and GolemAutomation.
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

    public GameObject heart;

    Tilting theBall;
    Rigidbody rb;


    public float boulderBarrageWidth = 22f;
    public float orbBarrageWidth = 22f;
    public float spawnWidth = 20f;
    public float spawnFrontOffset = 8f;

    // If it should look at the player
    [HideInInspector] public bool looking = true;

    // degrees per second
    public float turnSpeed;

    // unturned angle
    [HideInInspector] public float defaultAngle;

    [HideInInspector] public bool freezeRotation = false;

    // for phase 5
    public bool ringMovement;
    public float ringSpeed;
    public float riseSpeed;
    public float minHeight;
    public float maxBallHeight;
    public float radius;
    public GameObject pivotObject;



    List<GameObject> orbList = new List<GameObject>();

    public GolemState golemState = GolemState.Phase1;

    // Start is called before the first frame update
    void Start()
    {
        theBall = FindObjectOfType<Tilting>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    looking = !looking;
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    StartCoroutine(MoveSineRoutine(rb.position + Vector3.left * 30f, 10f));
        //}

    }

    void FixedUpdate()
    {
        RotateSelf();

    }

    void RotateSelf()
    {
        if (!freezeRotation)
        {
            Vector3 targetVec = looking ?
                (theBall.transform.position - rb.position)
                :
                GetDefaultFront();

            targetVec.y = 0;

            float angleDifference = Vector3.Angle(GetFront(), targetVec);

            float direction = (Vector3.Cross(GetFront(), targetVec).y > 0) ? 1f : -1f;

            float change = direction * Time.fixedDeltaTime * turnSpeed;

            if (Mathf.Abs(change) > angleDifference)
            {
                change = direction * angleDifference;
            }

            rb.MoveRotation(Quaternion.Euler(0f, rb.rotation.eulerAngles.y + change, 0f));

            if (ringMovement) { RingMovement();  }
        }
    }

    void RingMovement()
    {
        // pivot to ball
        Vector3 pivot = pivotObject.transform.position;
        Vector3 targetDir = theBall.transform.position - pivot;
        targetDir.y = 0f;

        float angleOffset = theBall.transform.position.y > maxBallHeight ? 0f : 30f;

        // target direction
        targetDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * targetDir;

        // current direction
        Vector3 currentDir = transform.position - pivot;
        currentDir.y = 0f;

        float angleDifference = VectorAngle(currentDir, targetDir);
        float ringDir = angleDifference > 0f ? 1f : -1f;
        float angleToMove = ringSpeed * Time.fixedDeltaTime * ringDir;
        if (Mathf.Abs(angleToMove) > Mathf.Abs(angleDifference)) { angleToMove = angleDifference; }

        Vector3 newDir = Quaternion.AngleAxis(angleToMove, Vector3.up) * currentDir;

        float targetHeight = theBall.transform.position.y;
        if (targetHeight < minHeight) { targetHeight = minHeight; }
        if (theBall.transform.position.y > maxBallHeight) { targetHeight += 3f; }

        float deltaHeight = targetHeight - rb.position.y;
        float heightToMove = riseSpeed * Time.fixedDeltaTime * (deltaHeight > 0 ? 1 : -1);
        if (Mathf.Abs(heightToMove) > Mathf.Abs(deltaHeight)) { heightToMove = deltaHeight; }
        float newHeight = rb.position.y + heightToMove;


        Vector3 where = pivot + newDir.normalized * radius;
        where.y = newHeight;
        rb.MovePosition(where);

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
        ShootOrb(orbList[0], RotateShiftVector(theBall.transform.position - orbList[0].transform.position, -0.75f));
        ShootOrb(orbList[1], RotateShiftVector(theBall.transform.position - orbList[1].transform.position, 0.75f));
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
        freezeRotation = true;

        Vector3 rightBeamDirection = RotateShiftVector(theBall.transform.position - rightEye.transform.position, 0.75f);
        Vector3 leftBeamDirection = RotateShiftVector(theBall.transform.position - leftEye.transform.position, -0.75f);

        GameObject rightBeam = Instantiate(beam);
        rightBeam.transform.position = rightEye.transform.position;
        rightBeam.transform.rotation = Quaternion.FromToRotation(Vector3.back, rightBeamDirection);


        GameObject leftBeam = Instantiate(beam);
        leftBeam.transform.position = leftEye.transform.position;
        leftBeam.transform.rotation = Quaternion.FromToRotation(Vector3.back, leftBeamDirection);

    }

    public void RecoverFromBeaming()
    {
        freezeRotation = false;
    }

    public void Look() { looking = true; }
    public void DoNotLook() { looking = false; }
    public void ClearOrbList() {
        foreach (GameObject orb in orbList)
        {
            orb.GetComponent<LightOrb>().Poof();
        }
        orbList.Clear();
    }

    public void DestroyBeams()
    {
        foreach (Beam beam in FindObjectsOfType<Beam>())
        {
            Destroy(beam.gameObject);
        }
    }

    public void Fall()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        freezeRotation = true;
    }

    public void Defeat()
    {
        freezeRotation = true;
        looking = false;
        ringMovement = false;
        heart.SetActive(false);
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

    public IEnumerator MoveSineRoutine(Vector3 to, float moveTime)
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
        looking = true;
    }

    // Find the golem's looking angle, from a direction vector
    float GolemAngleToLook(Vector3 to)
    {
        Vector3 from = Vector3.back;
        to.y = 0;

        return VectorAngle(from, to);

    }

    // where to move from "from" to "to", in terms of degrees 
    float VectorAngle(Vector3 from, Vector3 to)
    {
        from.y = 0;
        to.y = 0;

        float angle = Vector3.Angle(from, to);

        // Unity's Cross uses left hand rule... ok that clears the earlier confusion up
        return Vector3.Cross(from, to).y > 0 ? angle : -angle;
    }

}
