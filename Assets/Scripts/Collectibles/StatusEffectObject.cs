using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectObject : Triggerable
{
    public float maxTime;

    [System.NonSerialized] public StatusEffectType statusEffectType;

    // Inherit TriggerType
    public StatusEffectObject() { triggerType = TriggerType.StatusEffectObject; }

    public void GetCollected()
    {
        Destroy(gameObject);
    }
}

public enum StatusEffectType
{
    None,
    Magnet,
    Bouncy,
}

public class StatusEffectInfo
{
    public StatusEffectType type;
    public bool active;
    public float maxTime;
    public float elapsedTime;

    public StatusEffectInfo(StatusEffectType _type)
    {
        type = _type;
        active = false;
        maxTime = 0;
        elapsedTime = 0;
    }
}
