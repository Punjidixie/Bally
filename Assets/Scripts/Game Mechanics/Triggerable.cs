using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Triggerable : MonoBehaviour
{
 
    [System.NonSerialized] public TriggerType triggerType = TriggerType.None;
}

public enum TriggerType
{
    Crystal,
    WinBox,
    Switch,
    StatusEffectObject,
    None
}
