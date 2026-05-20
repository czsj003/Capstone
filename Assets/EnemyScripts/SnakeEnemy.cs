using System.Collections;
using UnityEngine;

public class SnakeEnemy : Enemy
{
    [SerializeField] private float jumpCooldown = 0.5f; // Jump cooldown
    [SerializeField] private float jumpHeight = 2f; // Jump height
    private bool isHurt = false;
    private bool isGrounded; // Tracks whether the snake is on the ground
    private float jumpCooldownTimer = 0f; // Timer to manage jump cooldown

    [SerializeField] private float bounceForce = 2f; // Bounce force
    [SerializeField] private float bounceDuration = 0.1f; // Bounce duration
    [SerializeField] private float knockbackDistance = 2f;
    [SerializeField] private float knockbackSpeed = 10f;

    private bool isBouncing = false;
    private int previousHP;

    [SerializeField] private GameObject fireballPrefab; // Reference to the Fireball prefab
    [SerializeField] private float fireballCooldown = 5f; // Cooldown time for fireball (5 seconds)
    private float fireballCooldownTimer = 0f; // Timer to manage fireball cooldown
    private bool isFiring = false; // Flag to check if fireball is being fired

    void Start()
    {
        base.Init();
        HP = 1000;
        Speed = 5;
        Threads = 100;
        DetectionRange = 10f;
        FacingRight = true;
        CollisionDamage = 10;
        int collisionKnockback = 5;
        collision = new DamageSource(CollisionDamage, transform, collisionKnockback, Effects ?? null);

        previousHP = HP; // Initialize previousHP to current HP
    }

    void Update()
    {
        // 处理HP减少的情况
        if (HP < previousHP)
        {
            PlayHurtAnimation(); // 播放受伤动画
            previousHP = HP;
        }

        // 处理火球发射
        if (fireballCooldownTimer <= 0 && !isFiring)
        {
            StartCoroutine(FireballRoutine()); // 发射火球并进入火球发射的协程
            FlipSprite();
        }
        else
        {
            fireballCooldownTimer -= Time.deltaTime;  // 更新冷却时间
        }

        // 发射火球时不能移动
        if (!isFiring)
        {
            MoveTowardsPlayer();
            FlipSprite();
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

            // 玩家在空中且冷却时间已过，执行跳跃
            if (Target.position.y > transform.position.y + 1f && isGrounded && jumpCooldownTimer <= 0)
            {
                Jump();
            }

            // 确保跳跃动画不被打断
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("SnakeJump") && !anim.GetCurrentAnimatorStateInfo(0).IsName("SnakeWalk"))
            {
                anim.SetTrigger("jumpTrigger");
            }
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    void Jump()
    {
        float jumpForceAdjusted = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y));
        rb.AddForce(new Vector3(0.0f, jumpForceAdjusted, 0.0f), ForceMode.Impulse);
        isGrounded = false; // 标记为不在地面上
        anim.SetTrigger("jumpTrigger"); // 播放跳跃动画
        jumpCooldownTimer = jumpCooldown; // 重置跳跃冷却时间
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
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
        rb.velocity = bounceDirection * bounceForce;
        yield return new WaitForSeconds(bounceDuration);
        rb.velocity = Vector3.zero;
        isBouncing = false;
    }

    private void PlayHurtAnimation()
    {
        if (!isHurt)
        {
            isHurt = true;
            anim.Play("SnakeHurt");
            StartCoroutine(ResetHurtAnimation());
        }
    }

    private IEnumerator ResetHurtAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
        if (isGrounded)
        {
            anim.SetBool("running", false);
        }
        else
        {
            anim.SetTrigger("jumpTrigger");
        }
    }

    private IEnumerator FireballRoutine()
    {
        // 发射火球
        isFiring = true; // 标记为正在发射火球
        ShootFireball();  // 发射火球
        fireballCooldownTimer = fireballCooldown;  // 重置火球冷却时间

        yield return new WaitForSeconds(2f); // 发射火球后停留2秒

        isFiring = false; // 恢复为可以移动状态
    }

    void ShootFireball()
    {
        Vector3 dir = Target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject fireball = Instantiate(fireballPrefab);
        fireball.transform.position = transform.position + new Vector3(0f, 0f, 0f);  // 偏移一点位置，让火球从一个稍微向右的地方发射
        Rigidbody fireballScript = fireball.GetComponent<Rigidbody>();
        fireballScript.useGravity = false;

        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
        fireballScript.velocity = direction * 15f;

        fireball.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool IsBeingKnockedBack()
    {
        return false; // Placeholder, you may implement knockback logic if needed
    }
}
