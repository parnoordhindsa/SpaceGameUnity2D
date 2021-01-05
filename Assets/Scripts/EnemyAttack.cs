using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject explosion;
    public float damage = 1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            return;
        }

        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        if (!(collider is null)) // if thing we hit should take damage
        {
            collider.TakeDamage(damage);
        }
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        GameObject soundEffect = Instantiate(explosion);
        Destroy(soundEffect, 1f);
        Destroy(effect, 0.4f);
        Destroy(gameObject);
    }

    public void ChangeDamageValue(float newDamage)
    {
        damage = newDamage;
    }
}
