using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IchorSlime : Enemy
{
    [SerializeField] private float jumpCooldown = 0.5f; // Jump cooldown
    [SerializeField] private float jumpHeight = 1.8f; // Jump height
    private bool isHurt = false;
    private bool isGrounded; // Tracks whether the slime is grounded
    private float jumpCooldownTimer = 0f; // Jump cooldown timer

    [SerializeField] private float bounceForce = 2f; // Bounce force
    [SerializeField] private float bounceDuration = 0.1f; // Bounce duration
    [SerializeField] private float knockbackDistance = 2f;
    [SerializeField] private float knockbackSpeed = 5f;

    private bool isBouncing = false;
    private int previousHP; // Used to track HP changes

    void Start()
    {
        // Fill in generic inherited values first
        //  refer to the Enemy and Entity Init() methods for the generic values
        base.Init();

        // Add any effects that this enemy applies on hit here


        // Fill out unique values for this enemy
        HP = 120;
        //Speed is unchanged
        DetectionRange = 5f;
        //FacingRight is unchanged
        CollisionDamage = 5;
        int collisionKnockback = 3;
        collision = new DamageSource(CollisionDamage, transform, collisionKnockback, Effects ?? null);

        // Fill out members unique to this enemy
        previousHP = HP; // Initialize previousHP to current HP
    }

    void Update()
    {
        // Check if HP has decreased and trigger the hurt animation
        if (HP < previousHP)
        {
            PlayHurtAnimation(); // Play hurt animation when HP decreases
            previousHP = HP; // Update previousHP to current HP
        }

        // Update jump cooldown timer
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (!IsBeingKnockedBack() && !isBouncing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

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

    void MoveTowardsPlayer()
    {
        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0; // 限制 Y 轴运动

        if (direction.magnitude > 0)
        {
            transform.position += direction * Speed * Time.deltaTime;
            anim.SetBool("running", true);

            // 检查玩家是否在高处并且在冷却时间之外
            if (Target.position.y > transform.position.y + 1f && isGrounded && jumpCooldownTimer <= 0)
            {
                Jump();
            }

            // 控制跳跃动画的播放
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("IchorJumpStartUp") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("IchorJumpLand"))
            {
                anim.SetTrigger("jumpTrigger"); // 触发跳跃动画
            }
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    void Jump()
    {
        // 计算跳跃力
        float jumpForceAdjusted = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y));
        rb.AddForce(new Vector3(0.0f, jumpForceAdjusted, 0.0f), ForceMode.Impulse);
        isGrounded = false; // 设置为不在地面上
        anim.SetTrigger("jumpTrigger"); // 触发跳跃动画
        jumpCooldownTimer = jumpCooldown; // 重置跳跃冷却时间
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Grounded when touching ground
        }

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

    private void PlayHurtAnimation()
    {
        // Play hurt animation if not already playing
        if (!isHurt)
        {
            isHurt = true;
            anim.Play("IchorHurt"); // Play hurt animation

            // Reset hurt state after animation
            StartCoroutine(ResetHurtAnimation());
        }
    }

    private IEnumerator ResetHurtAnimation()
    {
        // Wait for the hurt animation to complete
        yield return new WaitForSeconds(0.5f); // Adjust this to the duration of your hurt animation

        isHurt = false; // Reset hurt state

        // Optionally reset to idle or running animation
        if (isGrounded)
        {
            anim.SetBool("running", false);
        }
        else
        {
            anim.SetTrigger("jumpTrigger"); // Continue jump animation if in air
        }
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
        return false; // Currently no knockback logic implemented
    }
}
