using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireRight : MonoBehaviour
{
    // level 0 damage upgrade is this much
    public const float BASE_DAMAGE = 1.0f;
    // each level of damage upgrade multiplies previous level by this
    public const float DAMAGE_MULTIPLIER = BASE_DAMAGE / 5.0f;

    public GameObject hitEffect;
    public GameObject enemey1;
    public GameObject explosion;
    public float damage = BASE_DAMAGE;

    public void Initialize(PlayerUpgrades playerUpgrades)
    {
        damage = BASE_DAMAGE + DAMAGE_MULTIPLIER * playerUpgrades.damageLevel;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // This will ignore collision if the object is the player
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        } else
        {
            TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
            if (!(collider is null)) // thing we hit is something that should take damage
            {
                collider.TakeDamage(damage);
            }
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            GameObject soundEffect = Instantiate(explosion);
            Destroy(soundEffect, 1f);
            Destroy(effect, 0.4f);
            Destroy(gameObject);
        }
    }
}
