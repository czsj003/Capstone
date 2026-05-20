using UnityEngine;

public class SoundWave : Entity
{
    private Vector3 direction;   // 声波的飞行方向
    private float speed;         // 声波的飞行速度
    private int bounceCount = 0; // 反弹次数
    private int maxBounces = 5;  // 最大反弹次数


    void Start()
    {
        
    }

    public override void Kill()
    {
        
    }

    // 设置飞行方向和速度，dir 是目标方向向量
    public void SetDirectionAndSpeed(Vector3 dir, float s)
    {
        direction = dir.normalized;  // 归一化方向向量
        speed = s;

        // 旋转声波，使它朝向目标方向
        RotateTowardsTarget(dir);
    }

    void Update()
    {
        // 每帧更新声波的位置，按照给定的速度和方向飞行
        transform.position += direction * speed * Time.deltaTime;
    }

    // 使声波朝向目标方向旋转
    private void RotateTowardsTarget(Vector3 targetDirection)
    {
        // 计算目标方向的旋转角度
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // 设置 Z 轴旋转（让物体朝向目标方向）
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // 碰撞检测：当声波碰到固体物体时进行反弹处理
    private void OnCollisionEnter(Collision collision)
    {
        // 判断碰撞的物体是否属于 'SolidObject' 层
        if (collision.gameObject.layer == LayerMask.NameToLayer("SolidObject") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerPassable"))
        {
            if (bounceCount < maxBounces)
            {
                // 增加反弹次数
                bounceCount++;

                // 计算反射方向
                Vector3 reflectedDirection = Vector3.Reflect(direction, collision.contacts[0].normal);

                // 更新方向（反射后反转方向）
                direction = reflectedDirection.normalized;

                // 调整旋转使声波朝向新的反弹方向
                RotateTowardsTarget(direction);
            }
            else
            {
                // 达到最大反弹次数后销毁物体
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // 这里假设 Player 脚本有一个 Damage() 方法来减少 HP
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                DamageSource damageSource = new DamageSource(10, transform, 3, Effects ?? null);
                player.DamagePlayer(damageSource, player.effectHandler);
                Destroy(gameObject);  // 销毁声波
            }
        }
    }
}
