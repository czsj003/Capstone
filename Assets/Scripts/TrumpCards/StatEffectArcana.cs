using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffectArcana : PassiveArcana
{
    public List<Effect> Effects { get; private set; }
    [SerializeField] Effect.Serializable[] initializedEffects;

    private void Awake()
    {
        foreach (Effect effect in initializedEffects) { Effects.Add(effect); }
        initializedEffects = null;
    }

    override public void AddEffect(Player player) { foreach (Effect effect in Effects) player.effectHandler.ApplyPersistent(effect); }
    override public void RemoveEffect(Player player) { foreach (Effect effect in Effects) player.effectHandler.RemovePersistent(effect); }
}
