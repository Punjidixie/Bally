using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectTracker : MonoBehaviour
{
    //magnet
    public StatusEffectBox magnetTracker;

    //bouncy
    public StatusEffectBox bouncyTracker;
    
    public void Activate(StatusEffectType type, float maxTime)
    {
        StatusEffectBox box = EffectTypeToBox(type);
        box.slider.maxValue = maxTime;
        box.slider.value = maxTime;
        box.gameObject.SetActive(true);
        box.gameObject.transform.SetSiblingIndex(0);
        
    }

    public void Deactivate(StatusEffectType type)
    {
        StatusEffectBox box = EffectTypeToBox(type);
        box.gameObject.SetActive(false);

    }

    public void UpdateTime(StatusEffectType type, float time)
    {
        EffectTypeToBox(type).slider.value = time;
    }

    StatusEffectBox EffectTypeToBox(StatusEffectType type)
    {
        switch (type)
        {
            case StatusEffectType.Magnet:
                return magnetTracker;
            case StatusEffectType.Bouncy:
                return bouncyTracker;
            default:
                return magnetTracker;
        }
    }
}
