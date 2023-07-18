using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Triggerable
{
    BoxCollider boxCollider;

    public GameObject dangerBody;
    public GameObject warningBody;

    public float expandSpeed;
    public float shrinkSpeed;

    public float gracePeriod;
    public float fullSizePeriod;

    // Inherit TriggerType
    public Beam() { triggerType = TriggerType.Beam; }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        StartCoroutine(StartRoutine());
    }


    IEnumerator StartRoutine()
    {
        boxCollider.enabled = false;
        dangerBody.SetActive(false);
        yield return new WaitForSeconds(gracePeriod);
        StartCoroutine(ExpandRoutine());
    }

    IEnumerator ExpandRoutine()
    {
        dangerBody.SetActive(true);
        dangerBody.transform.localScale = Vector3.one * 0.1f;

        warningBody.SetActive(false);

        while (true)
        {
            float newScale = dangerBody.transform.localScale.x + expandSpeed * Time.deltaTime;

            if (newScale > 1f)
            {
                dangerBody.transform.localScale = Vector3.one;
                break;
            }

            dangerBody.transform.localScale = new Vector3(newScale, newScale, 1f);

            yield return null;
        }
        boxCollider.enabled = true;

        yield return new WaitForSeconds(fullSizePeriod);
        StartCoroutine(ShrinkRoutine());
        
    }

    IEnumerator ShrinkRoutine()
    {
        boxCollider.enabled = false;
        while (true)
        {
            float newScale = dangerBody.transform.localScale.x - shrinkSpeed * Time.deltaTime;

            if (newScale < 0.1f)
            {
                Destroy(gameObject);
                yield break;
            }

            dangerBody.transform.localScale = new Vector3(newScale, newScale, 1f);

            yield return null;
        }
    }
}
