using System.Collections;
using UnityEngine;

public class BatEnemy : Enemy
{
    [SerializeField] private float bounceForce = 2f; // Reduced force for a smaller bounce
    [SerializeField] private float bounceDuration = 0.1f; // Duration of the bounce effect
    [SerializeField] private float knockbackDistance = 2f;
    [SerializeField] private float knockbackSpeed = 5f;

    private bool isBouncing = false; // Track if the bat is currently bouncing

    void Start()
    {
        // Fill in generic inherited values first
        //  refer to the Enemy and Entity Init() methods for the generic values
        base.Init();

        // Add any effects that this enemy applies on hit here


        // Fill out unique values for this enemy
        HP = 50;
        Speed = 2;
        //Threads is unchanged
        DetectionRange = 5f;
        //FacingRight is unchanged
        CollisionDamage = 2;
        int collisionKnockback = 3;
        collision = new DamageSource(CollisionDamage, transform, collisionKnockback, Effects ?? null);
    }

    void Update()
    {
        if (!IsBeingKnockedBack() && !isBouncing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

            if (distanceToPlayer <= DetectionRange)
            {
                MoveTowardsPlayer();
                FlipSprite();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (!CanMove) return;

        Vector3 direction = (Target.position - transform.position).normalized;
        rb.velocity = direction * Speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BounceOffPlayer());
        }
    }

    private IEnumerator BounceOffPlayer()
    {
        isBouncing = true;

        Vector3 bounceDirection = (transform.position - Target.position).normalized;
        rb.velocity = bounceDirection * bounceForce; // Apply initial bounce force

        yield return new WaitForSeconds(bounceDuration); // Limit the bounce effect duration

        rb.velocity = Vector3.zero; // Stop the bat's movement after bounce
        isBouncing = false;
    }

    private IEnumerator HandleKnockback()
    {
        Vector3 knockbackDirection = (transform.position - Target.position).normalized;
        rb.velocity = knockbackDirection * knockbackSpeed;

        yield return new WaitForSeconds(knockbackDistance / knockbackSpeed);

        rb.velocity = Vector3.zero;
    }

    private bool IsBeingKnockedBack()
    {
        return false;
    }
}
