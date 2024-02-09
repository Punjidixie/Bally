using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAutomation : MonoBehaviour
{

    Animator animator;
    LevelController levelController;
    Golem golem;

    float t;

    bool actionFrozen = false;
    bool canTransition = true;

    // Game objects of where the golem should be in each phase
    public GameObject[] positionObjects;
    public float startT;
    public float startAngle;
    public GolemState startState;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        levelController = FindObjectOfType<LevelController>();
        golem = GetComponent<Golem>();
        golem.defaultAngle = startAngle;
        golem.golemState = startState;
        golem.freezeRotation = true;
        t = startT;
    }

    // Update is called once per frame
    void Update()
    {
        switch (levelController.levelState)
        {
            case "InGame":

                t -= Time.deltaTime;

                if (t <= 0)
                {
                    t = 0;
                    if (!actionFrozen)
                    {
                        DoSomething();
                    }
                }

                break;
            default:
                break;
        }
    }

    int i = 0;
    // Determine what to do, and then do it
    void DoSomething()
    {
        //string[] l = { "Beam", "DASH", "ShootTwo", "DASH", "LightBarrage", "ThrowTwo", "ThrowFive" };
        //float[] d = { 5f, 0f, 4f, 0f, 4f, 5f, 9f };


        //i = 2;
        //if (i != 1 && i != 3)
        //{
        //    TriggerAnimation(l[i], d[i]);
        //}
        //else if ( i == 1)
        //{
        //    StartCoroutine(DashTo(transform.position + Vector3.left * 50, 3, 3, 0f));
        //}
        //else if (i == 3)
        //{
        //    StartCoroutine(DashTo(transform.position + Vector3.right * 50, 3, 3, 0f));
        //}

        //i++;
        i++;
        switch (golem.golemState)
        {
            case GolemState.Phase1:
                ChoosePhase1Action(i);
                break;
            case GolemState.Phase2:
                ChoosePhase2Action(i);
                break;
            case GolemState.Phase3:
                ChoosePhase3Action(i);
                break;
            case GolemState.Phase4:
                TriggerAnimation("Beam", 6f);
                break;
            case GolemState.Phase5:
                TriggerAnimation("Beam", 6f);
                break;
            default:
                break;
        }
        
    }

    void ChoosePhase1Action(int seed)
    {
        if (seed % 2 == 0)
        {
            TriggerAnimation("ThrowTwo", 7f);
        }
        else
        {
            TriggerAnimation("ShootTwo", 7f);
        }
    }

    void ChoosePhase2Action(int seed)
    {
        if (seed % 2 == 0)
        {
            TriggerAnimation("ThrowFive", 13f);
        }
        else
        {
            TriggerAnimation("LightBarrage", 13f);
        }
    }

    void ChoosePhase3Action(int seed)
    {
        //string[] l = { "ShootTwo", "LightBarrage", "ThrowTwo", "ThrowFive" };
        //float[] d = { 4f, 8f, 4f, 12f };
        //TriggerAnimation(l[i % 4], d[i % 4]);

        if (seed % 2 == 0)
        {
            TriggerAnimation("ShootTwo", 15f);
        }
        else
        {
            TriggerAnimation("LightBarrage", 15f);
        }
    }



    void TriggerAnimation(string name, float timeTilNext)
    {
        animator.SetTrigger(name);
        t = timeTilNext;
    }

    IEnumerator StartRoutine()
    {
        golem.freezeRotation = false;
        golem.looking = true;
        animator.SetTrigger("Roar");
       
        yield return new WaitForSeconds(1.3f); // roar time

        golem.golemState = GolemState.Phase1;
        StartCoroutine(DashTo(positionObjects[0].transform.position, 5f, 3f, 0f));
    }

    public void LetsGo()
    {
        StartCoroutine(StartRoutine());
    }

    IEnumerator DashTo(Vector3 to, float moveTime, float timeAfterTilNext, float newDefaultAngle)
    {
        actionFrozen = true;
        TriggerAnimation("StartDash", 0);

        StartCoroutine(golem.MoveSineRoutine(to, moveTime));
        yield return new WaitForSeconds(moveTime);

        TriggerAnimation("StopDash", 0);
        golem.defaultAngle = newDefaultAngle;
        t = timeAfterTilNext;
        actionFrozen = false;
    }

    IEnumerator HurtDashTransition(GolemState newState, Vector3 newPosition, float moveTime, float timeAfterTilNext, float newDefaultAngle)
    {
        TriggerAnimation("Hurt", 0);

        // wait for the animation transition to die out
        yield return new WaitForSeconds(0.5f);
        golem.ClearOrbList();
        canTransition = false;
        actionFrozen = true;
        golem.golemState = newState;

        // wait for hurt to end
        yield return new WaitForSeconds(0.5f);
        golem.freezeRotation = false;
        

        canTransition = true;
        StartCoroutine(DashTo(newPosition, moveTime, timeAfterTilNext, newDefaultAngle));
    }

    IEnumerator Phase4ToPhase5()
    {
        // hurt
        TriggerAnimation("Hurt", 0);
        canTransition = false;
        actionFrozen = true;
        golem.golemState = GolemState.Phase5;
        yield return new WaitForSeconds(1);

        // hurt ends
        golem.freezeRotation = false;
        canTransition = true;

        Vector3 targetPos = golem.pivotObject.transform.position + Vector3.right * golem.radius;
        targetPos.y = golem.minHeight;

        // dash and wait til dash ends
        StartCoroutine(DashTo(targetPos, 1.5f, 1, 0f));
        yield return new WaitForSeconds(1.5f);
        golem.ringMovement = true;

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (canTransition)
        {
            switch (golem.golemState)
            {
                case GolemState.Phase1:
                    StartCoroutine(HurtDashTransition(GolemState.Phase2, positionObjects[1].transform.position, 5, 6, -90));
                    break;
                case GolemState.Phase2:
                    StartCoroutine(HurtDashTransition(GolemState.Phase3, positionObjects[2].transform.position, 5, 6, 180));
                    break;
                case GolemState.Phase3:
                    StartCoroutine(HurtDashTransition(GolemState.Phase4, positionObjects[3].transform.position, 5, 6, 90));
                    break;
                case GolemState.Phase4:
                    golem.DestroyBeams();
                    StartCoroutine(Phase4ToPhase5());
                    break;
                case GolemState.Phase5:
                    golem.DestroyBeams();
                    animator.SetTrigger("Defeat");
                    golem.Defeat();
                    break;
                default:
                    break;
            }
        }
       

        
    }

}
