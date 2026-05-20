using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigBatEnemy : Enemy
{
    public GameObject soundWavePrefab;   // 声波的 prefab
    public float soundWaveSpeed = 10f;   // 声波飞行的速度
    public float soundWaveCooldown = 2f; // 声波发射的冷却时间
    private float soundWaveTimer = 0f;   // 用来计时冷却

    public float minDistance = 5f;       // 蝙蝠和玩家之间的最小距离（攻击范围内）
    public float maxDistance = 8f;       // 蝙蝠和玩家之间的最大距离（最远距离）
    private bool isCharging = false;     // 是否正在冲刺攻击

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
        HP = 800;
        Speed = 10;
        Threads = 100;
        //DetectionRange is default
        FacingRight = true;
        CollisionDamage = 10;
        int collisionKnockback = 3;
        collision = new DamageSource(CollisionDamage, transform, collisionKnockback, Effects ?? null);
    }

    void Update()
    {
        if (!IsBeingKnockedBack() && !isBouncing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Target.position);

            // 如果玩家在检测范围内
            if (distanceToPlayer <= DetectionRange)
            {
                // 1. 在远距离，蝙蝠持续发射声波
                if (distanceToPlayer > maxDistance)
                {
                    // 发射声波
                    if (soundWaveTimer <= 0f)
                    {
                        FlipSprite();
                        FireSoundWave();
                        soundWaveTimer = soundWaveCooldown;  // 重置冷却时间
                    }
                    else
                    {
                        soundWaveTimer -= Time.deltaTime;  // 更新冷却时间
                    }
                }
                // 2. 在攻击范围内但不接近玩家，蝙蝠不主动靠近，只发射声波
                else if (distanceToPlayer > minDistance && distanceToPlayer <= maxDistance)
                {
                    // 发射声波
                    if (soundWaveTimer <= 0f)
                    {
                        FlipSprite();
                        FireSoundWave();
                        soundWaveTimer = soundWaveCooldown;  // 重置冷却时间
                    }
                    else
                    {
                        soundWaveTimer -= Time.deltaTime;  // 更新冷却时间
                    }
                }
                // 3. 如果距离小于 minDistance，蝙蝠冲刺攻击玩家
                else if (distanceToPlayer < minDistance)
                {
                    ChargeAtPlayer();
                    FlipSprite();
                }
            }
        }
    }

    // 发射声波
    void FireSoundWave()
    {
        Vector3 dir = Target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 实例化声波
        GameObject soundWave = Instantiate(soundWavePrefab);
        soundWave.transform.position = transform.position + new Vector3(0f, 0f, 0f);
        Rigidbody soundWaveScript = soundWave.GetComponent<Rigidbody>();
        soundWaveScript.useGravity = false;

        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
        soundWaveScript.velocity = direction * soundWaveSpeed;

        // 设置旋转
        soundWave.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 设置声音波的方向和速度
        SoundWave soundWaveEntity = soundWave.GetComponent<SoundWave>();
        if (soundWaveEntity != null)
        {
            soundWaveEntity.SetDirectionAndSpeed(direction, soundWaveSpeed); // 设置飞行方向和速度
        }
    }

    // 冲刺攻击玩家
    void ChargeAtPlayer()
    {
        if (isCharging) return;  // 如果已经在冲刺攻击中，避免重复进入冲刺

        isCharging = true;  // 设置冲刺状态
        Vector3 direction = (Target.position - transform.position).normalized;
        rb.velocity = direction * Speed * 2f;  // 提高速度进行冲刺

        // 设置冲刺持续时间，例如冲刺 1 秒后恢复正常
        StartCoroutine(StopChargingAfterDelay(1f));
    }

    // 停止冲刺
    IEnumerator StopChargingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCharging = false;
        rb.velocity = Vector3.zero;  // 停止冲刺后的速度
    }

    // 检测与 PlayerAttack Layer 的碰撞
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

    // 后退协程
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
