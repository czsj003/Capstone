using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player S;
    private AudioSource footsteps;
    public int roomPassed;
    public Whip whip;  // Need to replace this with a new abstract Weapon class to inherit our weapons from
    [SerializeField] private float jumpCooldown = 0.5f;
    private PlayerInput input;

    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f;

    private bool grounded = true;
    public HealthBar healthBar;

    public static List<Consumable> playerConsumables;

    // var for threads
    public static int threads;

    private int previousHP;


    private static float regenTime = 0;
    private static int regenCount = 0;
    private bool regenerating = false;

    void Start()
    {
        base.Init();

        S = this;   // Add error checking later

        footsteps = GetComponents<AudioSource>()[2];

        HP = 100;
        previousHP = HP;
        Speed = 12f;
        //Weight is unchanged
        healthBar.SetMaxHealth(HP);

        string currentSceneName = SceneManager.GetActiveScene().name;

        // 根据场景设置 roomPassed
        if (currentSceneName == "__Scene_L1")  // 比如场景0的名称是"Scene0"
        {
            roomPassed = 0;
        }
        else if (currentSceneName == "__Scene_L2")  // 比如场景1的名称是"Scene1"
        {
            roomPassed = 5;
        }
        else if (currentSceneName == "__Scene_L3")  // 比如场景1的名称是"Scene1"
        {
            roomPassed = 10;
        }
    }

    void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.input = GetComponent<PlayerInput>();
        playerConsumables = new List<Consumable>();
    }

    void FixedUpdate()
    {
        if (!CanMove) { //Debug.Log("Player cannot move");
                        footsteps.Stop(); return; }
        if (grounded && Mathf.Abs(rb.velocity.x) > 0.08f)
        {
            if (!footsteps.isPlaying) { footsteps.Play(); }
        }
        else { footsteps.Stop(); }

        Vector2 moveVector = input.actions.FindAction("Move").ReadValue<Vector2>();

        float moveSpeed = moveVector.x * Speed;

        // 如果玩家停滞超过一定时间，强制推动玩家
        if (rb.velocity.x == 0f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                // 强制给予一些水平速度，防止玩家卡住
                rb.velocity = new Vector3(0.1f, rb.velocity.y, 0);
                stuckTimer = 0f;  // 重置
            }
        }
        else
        {
            stuckTimer = 0f; // 如果玩家在移动，重置计时器
        }

        // 更新玩家的速度
        if (!float.IsNaN(moveSpeed) && Mathf.Abs(moveSpeed) < 1000f)
        {
            // Subtract current x velocity from the applied force to limit top speed
            //  Else statement reduces acceleration of the player when in the air so
            //  that the difference in speed is not too drastic
            if (grounded) rb.AddForce(new Vector3(moveSpeed - rb.velocity.x, rb.velocity.y, 0), ForceMode.Force);
            else rb.AddForce(new Vector3((moveSpeed - rb.velocity.x) * 0.5f, rb.velocity.y, 0), ForceMode.Force);
        }

        jumpCooldown -= Time.deltaTime;

        updateAnimationState(moveVector);

        if (regenCount > 0 && !regenerating)
        {
            regenerating = true;
            StartCoroutine(regen(regenTime));
            regenCount--;
        }

        CheckHPChange();

        // Kill checks are made in Damage() and DamagePlayer(), so we shouldn't need this
        //if (HP <= 0)
        //{
        //    Kill();
        //}
    }

    IEnumerator regen(float time)
    {
        Debug.Log("regen");
        yield return new WaitForSeconds(time);
        Player.S.HP += 1;
        regenerating = false;
        if (Player.S.HP > Player.S.MaxHP)
        {
            Player.S.HP = Player.S.MaxHP;
        }
    }
    private void CheckHPChange()
    {
        if (HP != previousHP)
        {
            healthBar.SetHealth(HP);
            previousHP = HP; // 更新 previousHP 为当前 HP
        }
    }

    public static void setRegen(float time, int totalHPToRestore)
    {
        regenTime = time;
        regenCount = totalHPToRestore;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && jumpCooldown < 0)
        {
            grounded = true;
        }
    }

    public void Jump()
    {
        if (grounded && CanMove)
        {
            grounded = false;
            jumpCooldown = 0.5f;
            rb.AddForce(new Vector3(0.0f, 6.0f, 0.0f), ForceMode.Impulse);
        }
    }

    private void updateAnimationState(Vector2 moveVector)
    {
        if (moveVector.x > 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = false;
        }
        else if (moveVector.x < 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = true;
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    public override void Kill()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOverScene");
    }

    // Move this to the Whip class
    // 检查 Whip 在当前方向上是否碰到障碍物
    private bool IsWhipCollidingWithObstacle(float moveDirection)
    {
        Vector3 whipDirection = moveDirection > 0 ? whip.transform.right : -whip.transform.right;  // 根据方向决定射线的方向
        RaycastHit hit;
        float whipLength = 1f;  // 根据 Whip 的实际长度设置

        // 射线检测
        if (Physics.Raycast(whip.transform.position, whipDirection, out hit, whipLength))
        {
            return true;  // 如果 Whip 碰到障碍物，返回 true
        }

        return false;  // 如果没有碰到障碍物，返回 false
    }

    public void consumeItem()
    {
        if (playerConsumables.Count > 0)
        {
            playerConsumables[0].consume();
        }
        else
        {
            FlavorText.flavorText.showText("\"I don't have any items.\"");
        }
    }

    public void damage(int damageHp)
    {
        HP -= damageHp;
    }

    // Static methods for weapons to retrieve player applications and effectHandler
    public static List<EffectApplier> GET_EFFECT_APPLIERS() { return S.Effects; }
    public static EffectHandler GET_EFFECT_HANDLER() { return S.effectHandler; }
}
