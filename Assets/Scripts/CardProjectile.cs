using UnityEngine;

public class CardProjectile : MonoBehaviour
{
    public DamageSource damageSource;
    public float speed;
    public int bounces;
    public int pierce;
    public Vector3 direction;

    private AudioSource bounce;

    // Initialize the projectile with card properties
    public void Initialize(int dmg, int kb, float spd, int bounceCount, int pierceCount, Vector3 dir)
    {
        damageSource = new DamageSource(dmg, transform, kb, null);
        speed = spd;
        bounces = bounceCount;
        pierce = pierceCount;
        direction = dir.normalized;
        bounce = GetComponent<AudioSource>();
        Debug.Log($"Initialized with Damage: {damageSource.Damage}, Knockback: {damageSource.Knockback}, Speed: {speed}, Bounces: {bounces}, Pierce: {pierce}");
    }

    void Update()
    {
        // Orient the cards
        float angle = Mathf.Atan2(transform.position.x + direction.x, transform.position.y + direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        // Move the projectile in the given direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        // This should now be redundant
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Card"))
        {
            return; // Do nothing, skip further logic
        }

        // Bounce if any bounces remain
        if (bounces > 0)
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal); // Reflect direction
            float angle = Mathf.Atan2(transform.position.x + direction.x, transform.position.y + direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            bounce.Play();
            bounces--;
            Debug.Log($"Bounce remaining: {bounces}");
        }
        else
        {
            Destroy(gameObject); // Destroy when bounces are exhausted
            Debug.Log("DESTROYED CARD");
        }

        // Handle pierce if colliding with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
            enemy.Damage(damageSource, Player.GET_EFFECT_HANDLER(), Player.GET_EFFECT_APPLIERS());
            pierce--;
            if (pierce <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
