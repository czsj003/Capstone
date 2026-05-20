using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : MonoBehaviour
{
    static private Whip S; // Singleton for weapon control

    public DamageSource whipDamage;

    public float maxRange = 4f; // 最大长度
    public float growthSpeed = 3f; // 增长速度
    public float shrinkSpeed = 3f; // 缩回速度
    public GameObject whipHead;

    private float initialScaleX = 0.12f; // 初始的 X 缩放值
    public enum State { Idle, Expanding, Shrinking }
    private State currentState = State.Idle;

    private Vector3 mouseDelta;
    private bool isColliding = false; // 碰撞状态

    private Collider whipCollider; // 用于禁用和启用碰撞器

    void Start()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogWarning("A singleton already exists for the weapon \"Whip\"");
        }

        // 确保 whipHead 的 Rigidbody 是 Kinematic
        Rigidbody whipRB = whipHead.GetComponent<Rigidbody>();
        if (whipRB != null)
        {
            whipRB.isKinematic = true; // 设置为 Kinematic
        }

        whipDamage = new DamageSource(30, transform, 3, null);

        // 获取并缓存 Whip 的 Collider
        whipCollider = GetComponent<Collider>();

        // 初始化 whipHead 的位置和缩放
        ResetWhip();
    }

    void Update()
    {
        // 只有在当前状态是 Idle 时，鼠标按下才会触发扩展动作
        if (currentState == State.Idle && Input.GetMouseButtonDown(0)) // 0 表示左键
        {
            currentState = State.Expanding;
            mouseDelta = GetMouseDirection();
        }

        if (currentState == State.Expanding) // 只有在没有碰撞的情况下才扩展
        {
            if (!CheckCollision()) // 检查是否与障碍物碰撞
            {
                ExtendWhip();
            }
            else
            {
                isColliding = true;
                currentState = State.Shrinking; // 开始缩回
            }
        }
        else if (currentState == State.Shrinking) // 处理缩回逻辑
        {
            ShrinkWhip();
        }
    }

    Vector3 GetMouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // 保持 Z 轴为 0，确保在 2D 平面内
        return mousePos - transform.position; // 计算方向
    }

    bool CheckCollision()
    {
        // 只有在非静止状态下才进行碰撞检测
        if (currentState == State.Idle)
            return false; // 不进行碰撞检测

        Vector3 currentDirection = mouseDelta.normalized;
        float currentLength = whipHead.transform.localScale.x;

        // 射线检测
        RaycastHit hit;
        if (Physics.Raycast(transform.position, currentDirection, out hit, currentLength))
        {
            return true; // 碰撞检测到物体
        }
        return false; // 没有碰撞
    }

    void ExtendWhip()
    {
        // 计算当前朝向的单位向量
        Vector3 currentDirection = mouseDelta.normalized;

        // 更新 whipHead 的大小，只改变 X 值
        float currentLength = whipHead.transform.localScale.x + (growthSpeed * Time.deltaTime);

        // 限制最大长度
        if (currentLength > maxRange)
        {
            currentLength = maxRange;
            currentState = State.Shrinking; // 达到最大长度后开始缩回
        }

        // 只修改 X 的缩放值
        whipHead.transform.localScale = new Vector3(currentLength, whipHead.transform.localScale.y, whipHead.transform.localScale.z);

        // 设置 whipHead 的 Z 旋转，使其指向鼠标方向
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        whipHead.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 固定位置为玩家位置，并基于当前方向和长度调整局部位置
        whipHead.transform.localPosition = new Vector3(currentDirection.x * (currentLength / 2), currentDirection.y * (currentLength / 2), 0);

        // 启用碰撞器
        EnableCollider(true);
    }

    void ShrinkWhip()
    {
        // 缩回逻辑
        float currentLength = whipHead.transform.localScale.x - (shrinkSpeed * Time.deltaTime);
        if (currentLength <= initialScaleX)
        {
            currentLength = initialScaleX; // 最小缩放值设为初始值
            currentState = State.Idle; // 缩回完成，状态回到 Idle
        }

        // 只修改 X 的缩放值
        whipHead.transform.localScale = new Vector3(currentLength, whipHead.transform.localScale.y, whipHead.transform.localScale.z);

        // 更新位置
        Vector3 currentDirection = mouseDelta.normalized;
        whipHead.transform.localPosition = new Vector3(currentDirection.x * (currentLength / 2), currentDirection.y * (currentLength / 2), 0);

        // 禁用碰撞器
        EnableCollider(false);
    }

    void ResetWhip()
    {
        whipHead.transform.localPosition = Vector3.zero;
        whipHead.transform.localScale = new Vector3(initialScaleX, whipHead.transform.localScale.y, whipHead.transform.localScale.z);

        // 禁用碰撞器
        EnableCollider(false);
    }

    // 启用或禁用 Whip 的 Collider
    void EnableCollider(bool enable)
    {
        if (whipCollider != null)
        {
            whipCollider.enabled = enable;
        }
    }

    // 返回当前 Whip 是否在扩展或收缩中
    public bool IsWhipActive()
    {
        return currentState == State.Expanding || currentState == State.Shrinking;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
            enemy.Damage(whipDamage, Player.GET_EFFECT_HANDLER(), Player.GET_EFFECT_APPLIERS());
        }
    }
}
