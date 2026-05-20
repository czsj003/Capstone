using UnityEngine;

public class Card
{
    protected int dmgMin, dmgMax;   // Damage range for the card
    protected int knockback;
    protected float speed;          // Card speed when thrown
    protected int bounces;          // Number of bounces before disappearing
    protected int pierce;           // Number of pierces before disappearing

    private Sprite _face;
    private Sprite _back;

    public Card(int dmgMin, int dmgMax, int knockback, float speed, int bounces, int pierce, Sprite face, Sprite back)
    {
        this.dmgMin = dmgMin;
        this.dmgMax = dmgMax;
        this.knockback = knockback;
        this.speed = speed;
        this.bounces = bounces;
        this.pierce = pierce;
        this.face = face;
        this.back = back;
    }

    // Public properties to access card attributes
    public int Knockback => knockback;
    public float Speed => speed;
    public int Bounces => bounces;
    public int Pierce => pierce;
    public Sprite face { get { return _face; } private set { _face = value; } }
    public Sprite back { get { return _back; } private set { _back = value; } }

    // Method to get random damage within range
    public int GetCardDamage() { return Random.Range(dmgMin, dmgMax + 1); }
}
