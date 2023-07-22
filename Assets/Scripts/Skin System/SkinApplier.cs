using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinApplier : MonoBehaviour
{

    Tilting tilting;
    public GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        tilting = GetComponent<Tilting>();

        ApplySkin();
    }

    // Update is called once per frame
    public void ApplySkin()
    {
        if (TopManager.skinPrefab)
        {
            Destroy(body);

            GameObject newBody = Instantiate(TopManager.skinPrefab);
            newBody.transform.SetParent(gameObject.transform);
            newBody.transform.localPosition = Vector3.zero;
            body = newBody;

            if (tilting) { tilting.body = body; }
        }
    }
}
