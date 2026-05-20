using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectApplyingArcana : PassiveArcana
{
    public List<EffectApplier> Effects { get; private set; }
    [SerializeField] private EffectApplier.Serializable[] initializedEffects;

    private void Awake()
    {
        foreach (EffectApplier effect in initializedEffects) { Effects.Add(effect); }
        initializedEffects = null;
    }

    override public void AddEffect(Player player) { foreach (EffectApplier effect in Effects) player.Effects.Add(effect); }
    override public void RemoveEffect(Player player) { foreach (EffectApplier effect in Effects) player.Effects.Remove(effect); }
}
