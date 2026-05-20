using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class TrumpCard : MonoBehaviour
{
    private SpriteRenderer trumpCardRenderer;
    [SerializeField] GameObject thisSide;
    [SerializeField] GameObject? otherSide; // May be null

    public Arcana Side1 { get { return thisSide.GetComponent<Arcana>(); } }
    public Arcana Side2 { get { if (otherSide != null) return otherSide.GetComponent<Arcana>(); else return null; } }
    public bool Reversible { get { return otherSide ? true : false; } } // return false if otherSide is null, true otherwise
    public bool ThisSidePassive { get { return Side1.Passive; } }
    // Don't mind the warning disable. I'm checking if otherSide is null in the get
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public bool OtherSidePassive { get { return Reversible ? Side2.Passive : false; } }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private void Start()
    {
        trumpCardRenderer = GetComponent<SpriteRenderer>();
    }

    public void Reverse(Player player)
    {
        if (Reversible)
        {
            if (ThisSidePassive)
            {
                // Remove the effect applied by the card
                Side1.RemoveEffect(player);
            }
            (otherSide, thisSide) = (thisSide, otherSide);

            // Check if the new active side is passive, apply effect if yes
            if (ThisSidePassive) Side1.AddEffect(player);
        }
    }
}
#nullable disable

abstract public class Arcana : MonoBehaviour
{
    [SerializeField] public bool Passive { get; protected set; }

    // ActiveArcana methods
    abstract public void UseItem();

    // PassiveArcana methods
    abstract public void AddEffect(Player player);
    abstract public void RemoveEffect(Player player);
}

abstract public class ActiveArcana : Arcana
{
    // Filter non-applicable methods
    override public void AddEffect(Player player) { }
    override public void RemoveEffect(Player player) { }
}
abstract public class PassiveArcana : Arcana
{
    // Filter non-applicable methods
    override public void UseItem() { }
}
