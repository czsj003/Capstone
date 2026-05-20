using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class Enemy : Entity //Parent class for all enemies. Contains shared variables.
{
    protected Transform Target { get; set; } // Reference to the player's position
    [SerializeField] protected int Threads { get; set; }  // Currency to be dropped on death
    [SerializeField] protected float DetectionRange { get; set; }
    [SerializeField] protected bool FacingRight { get; set; }
    public int CollisionDamage { get; set; }
    public DamageSource? collision { get; set; }
    
    override protected void Init()
    {
        // Fill default values for members inherited from Entity
        base.Init();

        // Fill out additional members added by Enemy
        //  these are generic values that can be overwritten in the specific subclasses if need be
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        Threads = 10;
        DetectionRange = 10;
        FacingRight = false;
        // This assumes that not every Enemy subclass will damage the player on direct contact
        CollisionDamage = 0;
        //Doing nothing with collision
    }

    public override void Kill()
    {
        // Drop threads
        if (Player.S != null)
        {
            Player.threads += Threads;
            Debug.Log($"Player threads: {Player.threads}"); // print new thread amount for testing
        }

        AudioManager.PLAY_DEATH_SOUND(deathSound.clip, transform.position);
        Destroy(gameObject);
    }

    public void Damage(DamageSource attackDamage, EffectHandler? playerEffectHandler)
    {
        // Apply damage to the enemy's HP
        HP -= attackDamage.Damage;

        // Apply knockback
        if (attackDamage.Knockback > 0)
        {
            // 计算击退的方向
            Vector3 knockbackDirection = (transform.position - attackDamage.Location.position).normalized;
            // Apply knockback force (assuming there's a Rigidbody component on the enemy)
            if (rb != null)
            {
                rb.AddForce(knockbackDirection * attackDamage.Knockback, ForceMode.Impulse);
            }
        }

        // If health is below or equal to 0, the enemy dies
        if (HP <= 0)
        {
            Kill();
        }
    }

    public void FlipSprite()
    {
        if (Target.position.x > transform.position.x && !FacingRight)
        {
            Flip();
        }
        else if (Target.position.x < transform.position.x && FacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Rotate the enemy 180 degrees on the Y axis, this avoids changing the scale and collider issues
        transform.Rotate(0f, 180f, 0f);
        FacingRight = !FacingRight;
    }
}
