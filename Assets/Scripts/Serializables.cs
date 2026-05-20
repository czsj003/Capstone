using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public struct SerializableEffect
{
    public string effectName;
    public string source;
    public EffectHandler.EffectName effectType;
    public float severity;
    public float time;
    public float cooldown;
    public float cdTime;
}
[Serializable] public struct SerializableEffectApplier
{
    public SerializableEffect effect;
    public float appChance;
}
[Serializable] public struct SerializableDamageSource
{
    public Transform transform;
    public int knockback;
    public List<SerializableEffectApplier> effects;
}
