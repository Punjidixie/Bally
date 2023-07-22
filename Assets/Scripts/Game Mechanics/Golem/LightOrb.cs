using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOrb : MonoBehaviour
{
    public float expansionRate;
    public GameObject sphere;

    bool fullSize = false;

    // Update is called once per frame
    void Update()
    {
        float s = sphere.transform.localScale.x + expansionRate * Time.deltaTime;
        if (s > 2f)
        {
            if (!fullSize)
            {
                sphere.transform.localScale = Vector3.one * 2f;
                fullSize = true;
            }
        }

        else if (!fullSize)
        {
            sphere.transform.localScale = Vector3.one * s;
        }
        
    }

    public void Poof()
    {
        Destroy(gameObject);
    }
}
