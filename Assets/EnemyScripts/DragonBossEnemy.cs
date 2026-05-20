using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossEnemy : Enemy
{
    [SerializeField] private float attackRange = 3f; // Range within which the dragon attacks the player

    void Start()
    {
        // Find the player by tag
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>(); // Get the Animator component

        HP = (200);
    }

    void Update()
    {
        // Check the distance between the dragon and the player
        float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

        // If the player is within detection range, move towards the player
        if (distanceToPlayer <= DetectionRange)
        {
            MoveTowardsPlayer();
            FlipSprite();

            // Check if the player is within attack range to trigger attack animation
            if (distanceToPlayer <= attackRange)
            {
                anim.SetBool("isAttacking", true); // Set attack animation
            }
            else
            {
                anim.SetBool("isAttacking", false); // Return to idle if out of attack range
            }
        }
        else
        {
            anim.SetBool("isAttacking", false); // Return to idle if out of detection range
        }
    }

    // Move the dragon towards the player's position only on the X axis
    void MoveTowardsPlayer()
    {
        Vector3 direction = (Target.position - transform.position).normalized; // Get the direction towards the player
        direction.y = 0; // Set Y to 0 to restrict movement to the X axis
        transform.position += direction * Speed * Time.deltaTime; // Move the dragon in that direction
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查 Layer 是否为 PlayerAttack
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            TakeDamage(2); // 从 hp 中减去 2
        }
    }

    // 处理受伤和死亡
    private void TakeDamage(int damage)
    {
        int currentHp = HP; // 假设你在 Enemy 类中有一个 HP 方法
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Destroy(gameObject); // 如果 hp 为 0，销毁物体
        }
        else
        {
            HP = (currentHp); // 更新 hp
            StartCoroutine(HandleKnockback()); // 启动后退协程
        }
    }

    // 后退协程
    private IEnumerator HandleKnockback()
    {
        // 计算后退方向（朝向 PlayerAttack 的方向）
        Vector3 knockbackDirection = (transform.position - Target.position).normalized;
        float knockbackDistance = 0.1f; // 后退的距离
        float knockbackSpeed = 1f; // 后退的速度

        float elapsed = 0f;
        while (elapsed < knockbackDistance / knockbackSpeed)
        {
            transform.position += knockbackDirection * knockbackSpeed * Time.deltaTime; // 后退
            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }
    }
}

