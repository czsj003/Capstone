using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable
public class EffectHandler : MonoBehaviour
{
    public enum EffectName { MaxHealth, CurrentHealth, Damage, Defense, Speed, Quality, Quantity }  // Debuffs use negative values

    List<Effect>? buffs;
    List<Effect>? debuffs;  // Separate buffs and debuffs to make cleansing easy.
    List<Effect>? persistent;   // Effects with indefinite duration (mostly passive cards)
    float invulnTime; // Prevents the entity from taking damage or being knocked back from any source.

    Entity entity;  // Stores the entity that this manager is attached to. Only used for recovery/dot effects.

    // Start is called before the first frame update
    void Start()
    {
        // Store a reference to the entity managed by this instance
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        float countdown = Time.deltaTime;

        // Tick down debuffs
        if (debuffs != null) TickDown(debuffs, countdown);

        // Tick down buffs
        if (buffs != null) TickDown(buffs, countdown);

        // Tick down invulnerability
        if (entity.Invulnerable)
        {
            invulnTime -= countdown;
            if (invulnTime <= 0) entity.Invulnerable = false;
        }
    }

    public void Apply(Effect effect)
    {
        if (effect.Severity < 0)    // Debuff
        {
            if (debuffs == null) { debuffs = new List<Effect> { effect }; }
            else CheckEffects(effect, debuffs);
        }
        else                        // Buff
        {
            if (buffs == null) { buffs = new List<Effect> { effect }; }
            else CheckEffects(effect, buffs);
        }
    }
    public void ApplyPersistent(Effect effect) 
    {
        if (persistent == null) { persistent = new List<Effect> { effect }; }
        else CheckEffects(effect, persistent);
    }
    public void RemovePersistent(Effect effect) {
        if (persistent == null)
        {
            Debug.LogWarning("No persistent effects to remove on "+entity.Name);
            return;
        }
        persistent.Remove(effect); 
    }
    /// <summary>
    /// Helper function for Apply.
    /// Checks an effect against the list of effects and refreshes the effect if already present, else applies the effect.
    /// </summary>
    /// <param name="newEffect"></param>
    /// <param name="effects"></param>
    void CheckEffects(Effect newEffect, List<Effect> effects)
    {
        // Refresh the present effect if newEffect is already in the effect list
        foreach (Effect effect in effects) {
            if (newEffect.Equals(effect)) {
                effect.Refresh(newEffect);
                return;
            }
        }
        // If not already present in the effect list
        effects.Add(newEffect);
    }

    /// <summary>
    /// Applies or refreshes the invulnerability effect on an entity
    /// </summary>
    /// <param name="invulnTime"></param>
    public void ApplyInvuln(float invulnTime)
    {
        entity.Invulnerable = true;
        invulnTime = Mathf.Max(invulnTime, invulnTime);
    }

    /// <summary>
    /// Counts down the remaining time of all effects in the list, and triggers any current health changes caused.
    /// </summary>
    /// <param name="effects">A list of effects to decrement the durations of.</param>
    /// <param name="time">The amount of time to decrement the effect durations by.</param>
    void TickDown(List<Effect> effects, float time)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            // Checks if the effect is recovery/dot and calls for it to affect the managed entity's health if off cooldown
            int hp = effects[i].TriggerRecovery(); if (hp != 0) entity.Recover(hp);
            // Decrement the effect duration
            effects[i].Countdown(time);
            if (effects[i].Time <= 0)
            {
                effects.RemoveAt(i);
                i--;    // Decrement i to account for element shift due to removal
            }
        }
    }

    /// <summary>
    /// Removes all debuffs on this entity.
    /// </summary>
    public void Cleanse() 
    {
        if (debuffs == null) { Debug.Log("No debuffs to cleanse on " + entity.Name); return; }
        debuffs.Clear();
    }

    /// <summary>
    /// Calculates the total of all buffs and debuffs for a particular effect.
    /// </summary>
    /// <param name="effect"></param>
    /// <returns>Returns the effectiveness multiplier for the stat as a percentage, or simply the total of all buffs/debuffs for flat changes.</returns>
    public float Modifier(EffectName effect)
    {
        float modifier = 100;
        if (effect == EffectName.CurrentHealth) modifier = 0; // Set modifier to 0 for calculating flat change in a stat
        if (buffs != null) foreach (Effect buff in buffs)
            if (buff.Type == effect) modifier += buff.Severity;
        if (debuffs != null) foreach (Effect debuff in debuffs)
            if (debuff.Type == effect) modifier -= debuff.Severity;
        if (persistent != null) foreach (Effect persistent in persistent)
            if (persistent.Type == effect) modifier += persistent.Severity;
        return modifier;
    }
}

// Shared container for an Effect and its chance to be applied on hit
public class EffectApplier
{
    [Serializable]
    public struct Serializable
    {
        public Effect.Serializable effect;
        [Tooltip("Treated as a decimal for calculations. Use a value ranging from (0, 1).")]
        public float appChance;
    }
    
    public Effect Effect { get; set; }
    public float AppChance { get; set; }

    public EffectApplier(Effect effect, float appChance) { Effect = effect; AppChance = appChance; }
    public EffectApplier(Serializable effectApplier) { Effect = effectApplier.effect; AppChance = effectApplier.appChance; }
    public static implicit operator EffectApplier(Serializable x) => new EffectApplier(x);
    /// <summary>
    /// Makes a random roll to determine if this effect should be applied.
    /// </summary>
    /// <returns>true if the effect should be applied, false otherwise</returns>
    public bool RollApplication() { return UnityEngine.Random.Range(0, 1) < AppChance; }
}


public class Effect : MonoBehaviour
{
    [Serializable] public struct Serializable
    {
        public string effectName;
        public string source;
        public EffectHandler.EffectName effectType;
        [Tooltip("Treated as a percent in calculations, except with Recovery/DoT effects. Use a positive value for buffs or negative for debuffs.")]
        public float severity;
        [Tooltip("The number of seconds to apply the effect for.")]
        public float time;
        [Tooltip("Amount of time between activations of the effect. Only used for health recovery or damage over time effects. Set to -1 if not using these.")] 
        public float cooldown;
        float cdTime;

        public float GetCDTime() { return cdTime; }
    }

    public string Name { get; private set; }
    public string Source { get; private set; }
    public EffectHandler.EffectName Type { get; private set; }
    public float Severity { get; private set; }
    public float Time { get; set; }

    // Track cooldowns on activations. Only used for recovery/dot effects
    public float Cooldown { get; private set; }
    public float CDTime { get; private set; }

    // Constructor for effects that do not change currentHP
    public Effect(string name, string source, EffectHandler.EffectName effect, float severity, float time)
    {
        Name = name;
        Source = source;
        Type = effect;
        Severity = severity;
        Time = time;

        Cooldown = -1;
    }
    // Constructor for effects that do change currentHP
    public Effect(string name, string source, EffectHandler.EffectName effect, float severity, float time, float cooldown)
    {
        Name = name;
        Source = source;
        Type = effect;
        Severity = severity;
        Time = time;

        Cooldown = cooldown;
    }
    
    // For compatibility with Serializable
    public Effect(Serializable effect)
    {
        Name = effect.effectName;
        Source = effect.source;
        Type = effect.effectType;
        Severity = effect.severity;
        Time = effect.time;
        Cooldown = effect.cooldown;
        CDTime = effect.GetCDTime();
    }
    public static implicit operator Effect(Serializable x) => new Effect(x);

    /// <summary>
    /// Checks if two buffs have the same name, source, and effect.
    /// </summary>
    /// <param name="other">The buff to compare with.</param>
    /// <returns>True if the names, sources, and effects of both buffs are equal, false otherwise.</returns>
    public bool Equals(Effect other)
    {
        if (other == null) return false;
        if (Name == other.Name && Source == other.Source && Type == other.Type) return true;
        else return false;
    }

    /// <summary>
    /// Reapplies an effect by taking the greater value between each buff's effectiveness and remaining duration.
    /// </summary>
    /// <param name="buff">The applying buff</param>
    public void Refresh(Effect buff)
    {
        // Effects increase in severity if refreshed with a stronger effect
        if (buff.Severity > Severity) Severity = buff.Severity;
        if (buff.Time > Time) Time = buff.Time;
    }

    public void Countdown(float time)
    {
        Time -= time;
        if (Cooldown != -1) // If the effect has a cooldown
            CDTime -= time;
    }
    
    public int TriggerRecovery()
    {
        if (Type != EffectHandler.EffectName.CurrentHealth || CDTime > 0) return 0;
        CDTime = Cooldown;
        return (int)Severity;
    }
}