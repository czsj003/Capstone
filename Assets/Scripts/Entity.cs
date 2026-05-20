using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

#nullable enable
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(EffectHandler))]
abstract public class Entity : MonoBehaviour
{
    protected Rigidbody rb;
    protected SpriteRenderer sprite;
    protected Animator anim;
    protected AudioSource hurtSound;
    protected AudioSource deathSound; 
    //[SerializeField] protected GameObject deathFX;
    public EffectHandler effectHandler;

    [SerializeField] public string Name { get; protected set; }
    [SerializeField] public int MaxHP { get; set; }
    public int HP { get; set; }
    [SerializeField] protected float Speed { get; set; }
    [SerializeField] protected float Weight { get; set; }
    public bool Invulnerable { get; set; }
    public List<EffectApplier>? Effects { get; set; } // List of effects that the entity can apply on hit and the chance for each to be applied
    // Private SerializeField only used to fill out the values for Effects
    [SerializeField] private List<EffectApplier.Serializable>? initializedEffects;
    public bool CanMove = true;

    private void Awake()
    {
        Init();
    }

    // Accessible initialization method to be called by inheriting classes
    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length < 2) { Debug.LogWarning($"{gameObject.name} does not have enough audio sources."); }
        else { 
            hurtSound = sources[0];
            deathSound = sources[1];
            //AudioSource deathFXSound = deathFX.GetComponent<AudioSource>();
            //deathFXSound = sources[1];
        }
        effectHandler = GetComponent<EffectHandler>();

        Name = gameObject.name;
        MaxHP = 100; HP = MaxHP;
        Speed = 3;
        Weight = 10;
        Invulnerable = false;

        if (initializedEffects != null)
        {
            Effects = new List<EffectApplier>();
            // Move the entire serializeEffects list to Effects
            foreach (EffectApplier effect in initializedEffects) { Effects.Add(effect); }
        }
    }

    public void Recover(int hp)
    {
        HP += hp;
        if (HP > MaxHP) HP = MaxHP;
        else if (HP <= 0) HP = 1; // Instead call Kill() if we don't want poison effects to leave at 1 hp

        // Call heal/damage number display
        DamageUI(hp);
    }

    // Separate function just to split the EffectApplier from the DamageSource so I don't have to rewrite the entire method for the player
    public void DamagePlayer(DamageSource source, EffectHandler sourceModifiers) {
        Damage(source, sourceModifiers, source.Effects);
    }
    public void Damage(DamageSource source, EffectHandler sourceModifiers, List<EffectApplier>? sourceEffects)
    {
        // Prevent damage to invulnerable entities
        if (Invulnerable) { return; }

        // Temporary variable for invulnerability time when hit.
        // Might add to Entity class for the sake of item effect overrides later.
        float invTime = 1.0f;

        // Calculation is: (int)(damage * source.damageMultiplier - this.defenseMultiplier)
        HP -= (int)(source.Damage * (sourceModifiers.Modifier(EffectHandler.EffectName.Damage) / 100) -
            (effectHandler.Modifier(EffectHandler.EffectName.Defense) / 100));
        // Call heal/damage number display
        DamageUI(source.Damage);
        // Roll all effects from the source if any exist and apply any that succeed

        if (sourceEffects != null) foreach (EffectApplier effect in sourceEffects)
            if (effect.RollApplication()) effectHandler.Apply(effect.Effect);

        
        Debug.Log("Dealt " + source.Damage + " damage to " + Name);
        // Call knockback coroutine
        StartCoroutine(HandleKnockback(source));
        // Check if entity should die, if not: apply invulnerability for <invTime> seconds
        if (HP <= 0) Kill();
        else {
            hurtSound.Play();
            effectHandler.ApplyInvuln(invTime);
        }
    }
    // Definitely will NOT stay here. I wrote this here for now to prevent errors keeping us from testing.
    public void DamageUI(int damage) { Color color = (damage < 0 ? Color.red: Color.green); }

    abstract public void Kill();

    protected void OnDestroy()
    {
        
    }

    /// <summary>
    /// Calculates the knockback caused to one entity by another using their respective weight and knockback power
    /// </summary>
    /// <param name="source">The source causing the knockback</param>
    /// <returns></returns>
    private IEnumerator HandleKnockback(DamageSource source)
    {
        Transform thisEntity = GetComponent<Transform>();
        // 计算后退方向（朝向 PlayerAttack 的方向）
        int knockbackDirection = (thisEntity.position.x < source.Location.position.x) ? -1 : 1;

        /*##THIS NEEDS TO BE CHANGED##*/
        // Calculations for knockback distance and speed may not be final
        //float knockbackDist = Weight / source.Knockback;
        //float knockbackSpeed = Weight - (Weight / source.Knockback);
        float kbVelocity = (Weight / source.Knockback);


        float elapsed = 0f; //#This can probably still be used
        //#Entities can still push other entities out of the map from here
        //  Lock Entity movement and launch them at a 45deg angle in 
        //  knockbackDirection with velocity = knVelocity
        CanMove = false;    //not working: player can still move during knockback
        rb.velocity = new Vector3(knockbackDirection * kbVelocity, kbVelocity, 0);  //no velocity in the x axis for some reason
        while (elapsed < 1f)
        {
            //thisEntity.position += knockbackDirection * knockbackSpeed * Time.deltaTime; // 后退
            elapsed += Time.deltaTime;
            //yield return null; // 等待下一帧
        }
        CanMove = true;
        yield return null;
        /*############################*/
    }
}
