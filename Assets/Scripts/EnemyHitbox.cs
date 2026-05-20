using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] public Enemy thisEnemy { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        thisEnemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 3: // The player
                other.GetComponent<Player>().DamagePlayer(thisEnemy.collision, thisEnemy.effectHandler);
                Debug.Log("Player attacked by " + thisEnemy.Name + " from Direct Collision");
                break;
            case 7: // PlayerAttack
                // Check if the collided object is a CardProjectile
                CardProjectile cardProjectile = other.gameObject.GetComponent<CardProjectile>();

                if (cardProjectile != null)
                {
                    Debug.Log("Card damage: " + cardProjectile.damageSource);
                    // Get information from the CardProjectile
                    DamageSource cardDamageSource = cardProjectile.damageSource;

                    // Call Damage method on the enemy, passing in the attack data
                    thisEnemy.Damage(cardDamageSource, Player.GET_EFFECT_HANDLER(), Player.GET_EFFECT_APPLIERS());

                    // Handle pierce logic: if the projectile is out of pierces, destroy it
                    cardProjectile.pierce--;
                    if (cardProjectile.pierce <= 0)
                    {
                        Destroy(other.gameObject); // Destroy the card projectile if it has no pierces left
                        Debug.Log("Card projectile destroyed due to pierce count reaching 0");
                    }
                }
                break;
            case 11: //WhipAttack
                Whip scarf = other.gameObject.GetComponent<Whip>();

                if (scarf != null)
                {
                    Debug.Log("Scarf damage: " + scarf.whipDamage);
                    thisEnemy.Damage(scarf.whipDamage, Player.GET_EFFECT_HANDLER(), Player.GET_EFFECT_APPLIERS()); // Assuming no specific effect handler for the attack
                }
                break;
        }
    }
}
