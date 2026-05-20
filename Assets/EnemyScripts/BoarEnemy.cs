using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarEnemy : Enemy
{
    [SerializeField] private float bounceForce = 2f; // Bounce force
    [SerializeField] private float bounceDuration = 0.1f; // Bounce duration
    [SerializeField] private float knockbackDistance = 2f;
    [SerializeField] private float knockbackSpeed = 5f;

    private bool isBouncing = false;
    void Start()
    {
        // Fill in generic inherited values first
        //  refer to the Enemy and Entity Init() methods for the generic values
        base.Init();

        // Add any effects that this enemy applies on hit here


        // Fill out unique values for this enemy
        HP = 100;
        Speed = 5;
        //Threads is unchanged
        DetectionRange = 10f;
        FacingRight = true;
        CollisionDamage = 10;
        int collisionKnockback = 5;
        collision = new DamageSource(CollisionDamage, transform, collisionKnockback, Effects ?? null);
    }

    void Update()
    {
        if (!IsBeingKnockedBack() && !isBouncing)
        {
            // Check the distance between the boar and the player
            float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

            // If the player is within detection range, move towards the player
            if (distanceToPlayer <= DetectionRange)
            {
                MoveTowardsPlayer();
                FlipSprite();
            }
            else
            {
                anim.SetBool("running", false);
            }
        }
    }

    // Move the boar towards the player's position only on the X axis
    void MoveTowardsPlayer()
    {
        Vector3 direction = (Target.position - transform.position).normalized; // Get the direction towards the player
        direction.y = 0; // Set Y to 0 to restrict movement to the X axis

        if (direction.magnitude > 0)
        {
            transform.position += direction * Speed * Time.deltaTime;
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
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
        rb.velocity = bounceDirection * bounceForce; // Apply bounce force

        yield return new WaitForSeconds(bounceDuration); // Limit bounce effect duration

        rb.velocity = Vector3.zero; // Stop movement after bounce
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
