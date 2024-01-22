using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public StatusType type;
    public float remainingDuration;
    public float intensity;

    public StatusEffect(StatusType _type, float _duration, float _intensity)
    {
        this.type = _type;
        this.remainingDuration = _duration;
        this.intensity = _intensity;
    }
}
