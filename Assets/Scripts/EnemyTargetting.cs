using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetting : MonoBehaviour
{
    private GameObject player;
    public GameObject enemyFire;
    public GameObject enemyFirePoint;

    private float distanceToPlayer;
    public int timeToShoot;
    public float enemyAttackRange;
    private float enemySightRange = 20f;
    public float attackLifeTime = 2f;
    public float bulletSpeed = 20f;
    public float enemyDamage;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer < enemyAttackRange && timeToShoot >= 45)
        {
            EnemyShoot();
            timeToShoot = 0;
        }
    }

    private void FixedUpdate()
    {
        timeToShoot++;
        if (distanceToPlayer < enemySightRange) // Aim at either the player or the direction they are moving
        {
            EnemyAiming(player.transform.position);
        }
        else
        {
            //EnemyAiming(gameObject.GetComponent<EnemyMovement>().tmpTarget);
        }
    }

    private void EnemyShoot()
    {
        GameObject enemyAttack = Instantiate(enemyFire, enemyFirePoint.transform.position, enemyFirePoint.transform.rotation);
        Rigidbody2D rb = enemyAttack.GetComponent<Rigidbody2D>();
        rb.AddForce(enemyFirePoint.transform.up * bulletSpeed, ForceMode2D.Impulse);
        enemyAttack.GetComponent<EnemyAttack>().ChangeDamageValue(enemyDamage);
        Destroy(enemyAttack, attackLifeTime);
    }

    private void EnemyAiming(Vector3 position)
    {
        Vector2 lookDir = position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ChangeScalingVars(float newRange, float newDamage, int newAttackSpeed, float newProjSpeed)
    {
        timeToShoot = newAttackSpeed;
        enemyAttackRange = newRange;
        enemyDamage = newDamage;
        bulletSpeed = newProjSpeed;
    }
}
