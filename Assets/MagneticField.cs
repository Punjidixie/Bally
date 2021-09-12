using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    float pullForce;
    float pullSpeed;
    public GameObject owner;
    public HashSet<GameObject> crystalsInEffect = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //pullForce = 30;

        pullSpeed = 10;
    }

   
    void LateUpdate()
    {

        transform.position = owner.transform.position;
    }

    private void OnDisable()
    {
        StopCrystalsInEffect();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            crystalsInEffect.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
           
            Vector3 direction = transform.position - other.gameObject.transform.position;
            direction.Normalize();
            //other.gameObject.GetComponent<Rigidbody>().AddForce(direction * pullForce, ForceMode.Acceleration);
            other.gameObject.GetComponent<Rigidbody>().velocity = direction * pullSpeed;

            //do not attract if not InGame
            if (owner.GetComponent<Tilting>().levelController.levelState != "InGame")
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            crystalsInEffect.Remove(other.gameObject);
        }
    }

    void StopCrystalsInEffect()
    {
        foreach (GameObject crystal in crystalsInEffect)
        {
            if (crystal != null)
            {
                crystal.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
}
