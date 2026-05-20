using UnityEngine;

public class Fireball : Entity
{
    void Start()
    {
        // 可以初始化一些默认值，或者不需要
    }

    public override void Kill()
    {
        // 如果需要，可以在火球销毁时执行一些特效或清理工作
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SolidObject") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerPassable"))
        {
            Destroy(gameObject); // 撞到固体物体销毁火球
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                DamageSource damageSource = new DamageSource(10, transform, 3, Effects ?? null);
                player.DamagePlayer(damageSource, player.effectHandler); // 伤害玩家
                Destroy(gameObject); // 碰撞后销毁火球
            }
        }
    }
}
