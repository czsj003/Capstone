using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    static private ItemHandler S;   // might decide to remove this
    private void Start() { S = this; }

    [SerializeField] AudioSource useSound; // Plays on item use
    [SerializeField] AudioSource reverseSound; // Plays when the card is reversed
    [SerializeField] AudioSource actionFailSound;

    [SerializeField] Player player; // Player reference for EffectHandler and EffectApplier access
    [SerializeField] public int PassiveSlots { get; private set; }
    [SerializeField] public int ActiveSlots { get; private set; }
    List<TrumpCard> activeTrumps = new List<TrumpCard>();
    List<TrumpCard> passiveTrumps = new List<TrumpCard>();

    public void UseItem() { }

    /// <summary>
    /// Attempts to reverse the given card, changing its effect.
    /// Not all cards are reversible, and reversal will fail if there are not enough inventory slots of the reversed card's type.
    /// </summary>
    /// <param name="card">The card to reverse.</param>
    static public void Reverse(TrumpCard card)
    {
        if (card.Reversible)
        {
            if (card.ThisSidePassive)
            {
                // Not enough space (Passive -> Active)
                if (!card.OtherSidePassive && S.activeTrumps.Count >= S.ActiveSlots) 
                { S.FailReverse(card); return; }

                // Remove the old card from the passive item list
                S.passiveTrumps.Remove(card);
            }
            else // Side 1 is Active
            {
                // Not enough space (Active -> Passive)
                if (card.OtherSidePassive && S.passiveTrumps.Count >= S.PassiveSlots) 
                { S.FailReverse(card); return; }

                // Remove the old card from the consumable item list
                S.activeTrumps.Remove(card);
            }
            // Reverse the card
            card.Reverse(S.player);
            // Add the reversed card to the correct item slot
            if (card.ThisSidePassive) S.passiveTrumps.Add(card); else S.activeTrumps.Add(card);
        }
    }
    private void FailReverse(TrumpCard card)
    {
        // Animate: shake (rotate) the card in it's slot

        // Play fail sound effect
        S.actionFailSound.Play();
    }
    
}

