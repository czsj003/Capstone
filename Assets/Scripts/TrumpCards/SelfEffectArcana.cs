using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfEffectArcana : ActiveArcana
{
    public Player player;
    public List<Effect> Effects { get; private set; }
    [SerializeField] Effect.Serializable[] initializedEffects;

    private void Awake()
    {
        foreach (Effect effect in initializedEffects) { Effects.Add(effect); }
        initializedEffects = null;
        player = FindObjectOfType<Player>();
        if (player == null) { Debug.LogError("No player instance detected for SelfEffectArcana"); }
    }

    public override void UseItem()
    {
        foreach(Effect effect in Effects) { player.effectHandler.Apply(effect); }
    }
}
