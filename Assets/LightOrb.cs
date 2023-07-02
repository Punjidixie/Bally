using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOrb : MonoBehaviour
{
    public float expansionRate;
    public GameObject sphere;

    bool fullSize = false;

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        float s = sphere.transform.localScale.x + expansionRate * Time.deltaTime;
        if (s > 2.5f)
        {
            if (!fullSize)
            {
                sphere.transform.localScale = Vector3.one * 2.5f;
                fullSize = true;
            }
        }

        else if (!fullSize)
        {
            sphere.transform.localScale = Vector3.one * s;
        }
        
    }
}
