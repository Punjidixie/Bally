using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : Triggerable
{

    public bool canRetrigger;
    public bool switchOn;

    public UnityEvent onSwitchOn;
    public UnityEvent onSwitchOff;

    public GameObject arch0;
    public GameObject arch1;
    public GameObject arch2;
    public GameObject rotator;

    public GameObject cones;
    public GameObject spotlight;

    public Material onMaterial;
    public Material offMaterial;

    public GameObject switchSound;

    float transitionTime;
    float conesTiltAngle;
    //float spotlightHighIntensity;
    //float spotlightLowIntensity;

    // Inherit TriggerType
    public Switch() { triggerType = TriggerType.Switch; }

    // Start is called before the first frame update
    void Start()
    {
        switchOn = false;
        transitionTime = 0.5f;
        conesTiltAngle = 30f;
        //spotlightHighIntensity = 30f;
        //spotlightLowIntensity = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerSwitch()
    {
        if (canRetrigger)
        {
            if (!switchOn)
            {
                TurnOn();
            }

            else
            {
                TurnOff();
            }

            GameObject s = Instantiate(switchSound);
            Destroy(s, 5);
        }
        else if (!canRetrigger)
        {
            if (!switchOn)
            {
                TurnOn();
                GameObject s = Instantiate(switchSound);
                Destroy(s, 5);
            }
        }
    }

    public void TurnOn()
    {
        StartCoroutine(ArchUpCoroutine());
        onSwitchOn.Invoke();
        switchOn = true;
    }

    public void TurnOff()
    {
        StartCoroutine(ArchDownCoroutine());
        onSwitchOff.Invoke();
        switchOn = false;
    }

    IEnumerator ArchUpCoroutine()
    {
        rotator.GetComponent<JustRotate>().enabled = true;
        //Light l = spotlight.GetComponent<Light>();

        cones.transform.GetChild(0).GetComponent<MeshRenderer>().material = onMaterial;

        float t = 0;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            arch0.transform.localPosition = Vector3.Lerp(new Vector3(arch0.transform.localPosition.x, 0.125f, arch0.transform.localPosition.z), new Vector3(arch0.transform.localPosition.x, 0.625f, arch0.transform.localPosition.z), t / transitionTime);
            arch1.transform.localPosition = Vector3.Lerp(new Vector3(arch1.transform.localPosition.x, 0.125f, arch1.transform.localPosition.z), new Vector3(arch1.transform.localPosition.x, 0.5f, arch1.transform.localPosition.z), t / transitionTime);
            arch2.transform.localPosition = Vector3.Lerp(new Vector3(arch2.transform.localPosition.x, 0.125f, arch2.transform.localPosition.z), new Vector3(arch2.transform.localPosition.x, 0.375f, arch2.transform.localPosition.z), t / transitionTime);

            cones.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(conesTiltAngle, 0, 0), t / transitionTime);
            
            //l.intensity = Mathf.Lerp(spotlightLowIntensity, spotlightHighIntensity, t / transitionTime);
            yield return null;
        }
    }

    IEnumerator ArchDownCoroutine()
    {
        rotator.GetComponent<JustRotate>().enabled = false;
        //Light l = spotlight.GetComponent<Light>();


        cones.transform.GetChild(0).GetComponent<MeshRenderer>().material = offMaterial;

        float t = 0;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            arch0.transform.localPosition = Vector3.Lerp(new Vector3(arch0.transform.localPosition.x, 0.625f, arch0.transform.localPosition.z), new Vector3(arch0.transform.localPosition.x, 0.125f, arch0.transform.localPosition.z), t / transitionTime);
            arch1.transform.localPosition = Vector3.Lerp(new Vector3(arch1.transform.localPosition.x, 0.5f, arch1.transform.localPosition.z), new Vector3(arch1.transform.localPosition.x, 0.125f, arch1.transform.localPosition.z), t / transitionTime);
            arch2.transform.localPosition = Vector3.Lerp(new Vector3(arch2.transform.localPosition.x, 0.375f, arch2.transform.localPosition.z), new Vector3(arch2.transform.localPosition.x, 0.125f, arch2.transform.localPosition.z), t / transitionTime);
            cones.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(conesTiltAngle, 0, 0), Quaternion.Euler(0, 0, 0), t / transitionTime);
            //l.intensity = Mathf.Lerp(spotlightHighIntensity, spotlightLowIntensity, t / transitionTime);
            yield return null;
        }

        
 
    }
}
