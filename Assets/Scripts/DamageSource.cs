using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class DamageSource
{
    [Serializable] public struct Serializable
    {
        public int damage;
        public Transform location;
        public int knockback;
        [Tooltip("Ignore this field. Put all effect applier objects in Initialized Effects.")]
        public List<EffectApplier>? effects;
        public List<EffectApplier.Serializable> initializedEffects;
    }

    public int Damage { get; private set; }
    public Transform Location { get; set; }    // The position of the object producing this damage source
    public int Knockback { get; set; }  // The amount of knockback caused by this source
    public List<EffectApplier>? Effects { get; private set; }  
    [SerializeField] private EffectApplier.Serializable[]? initializedEffects;

    public DamageSource(int damage, Transform transform, int knockback, List<EffectApplier>? effects)
    {
        Damage = damage;
        Location = transform;
        Knockback = knockback;
        if (effects != null) Effects = effects;
    }
    public DamageSource(Serializable source) 
    {
        Damage = source.damage;
        Location = source.location;
        Knockback = source.knockback; 
        if (source.effects != null)
        {
            Effects = new List<EffectApplier> { Capacity = source.effects.Capacity };
            foreach (EffectApplier effect in source.effects) { Effects.Add(effect); }
        }
    }
    public static implicit operator DamageSource(Serializable x) => new DamageSource(x);

    override public string ToString()
    {
        string s = "Dealing " + Damage + " with " + Knockback + " knockback and " + (Effects == null ? "no" : Effects.Count) + " effects to roll.";
        return s;
    }
}
