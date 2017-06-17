using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffs : MonoBehaviour {

    [SerializeField]
    protected float duration;
    public float Duration { get { return duration; } }
    protected float currentDuration;

    public void OnActivate () { }

    public virtual void MovementUpdate ( ref Vector2 VelocityMod ) { }

    public void HealthUpdate () { }
}
