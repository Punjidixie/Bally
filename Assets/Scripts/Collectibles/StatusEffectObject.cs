using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectObject : Triggerable
{
    public float maxTime;

    [System.NonSerialized] public StatusEffectType statusEffectType;

    // Inherit TriggerType
    public StatusEffectObject() { triggerType = TriggerType.StatusEffectObject; }
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
}
