using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, TakesDamage
{
    public Vector3 curPosition;
    public Vector2 tmpDimension;
    public Vector3 tmpTarget;

    [System.NonSerialized]
    public bool target;
    [System.NonSerialized]
    public float speed;

    public GameObject deathSoundEffect;

    public GameObject deathEffect;

    public float HitPoints;
    public float MaxHitPoints = 5;
    public EnemyHealth healthBar;

    // Scaling vars

    private int[] healthValues = { 5, 8, 13, 20, 35 };
    private float[] damageValues = { 1f, 1.5f, 2f, 2.5f, 3f };
    private float[] speedValues = { 0.05f, 0.06f, 0.07f, 0.08f, 0.08f };
    private Color[] colorValues = { new Color32(255, 255, 255, 255),new Color32(120, 160, 255, 255),
        new Color32(120, 255, 180, 255), new Color32(255, 120, 140, 255), new Color32(120, 60, 65, 255) };
    private int[] attackSpeedValues = { 45, 40, 35, 30, 25 };
    private float[] attackRangeValues = { 11f, 11.5f, 12.5f, 13.5f, 15f };
    private float[] projectileSpeed = { 20f, 22f, 25f, 29f, 35f };

    // Pathfinding vars

    public Rigidbody2D rb;
    private GameObject player;
    private Stack<Vector3> path;
    private int pathfindingDelay;
    private int pathfindingRandom;
    private int pathingState;
    private Vector3 pathfindingTarget;
    private float distanceToPlayer;

    [SerializeField]
    public float combatAIDistance = 9f;
    [SerializeField]
    public float sleepAIDistance = 50f;

    void Start()
    {
        curPosition = transform.position;
        tmpDimension = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        //tmpTarget = new Vector3(Random.Range(tmpDimension.x * -1, tmpDimension.x), Random.Range(tmpDimension.y * -1, tmpDimension.y), 0);
        target = false;


        // HealthBar

        HitPoints = MaxHitPoints;
        healthBar.SetMaxHealth(MaxHitPoints);

        // Pathfinding

        player = GameObject.FindGameObjectWithTag("Player");
        path = new Stack<Vector3>();
        pathfindingRandom = Random.Range(30, 60);
        pathfindingTarget = transform.position;
        pathfindingDelay = 100;
    }

    public void TakeDamage(float damage)
    {
        HitPoints -= damage;
        healthBar.SetHealth(HitPoints);

        if (HitPoints <= 0)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameObject deathSound = Instantiate(deathSoundEffect);
            Destroy(deathSound, 1.5f);
            Destroy(effect, 0.5f);

            // drop random loot
            GameObject resourcePrefab = GameObject.FindWithTag("Level").GetComponent<Level>().EnemyDrops.Get();
            Instantiate(resourcePrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // enemy collisions are ignored
        if (collision.collider.gameObject.tag == "Enemy")
            return;

        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        collider?.TakeDamage(1.0f);
    }

    private void FixedUpdate()
    {
        if (pathfindingDelay > pathfindingRandom)
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer < combatAIDistance) // Combat AI
            {
                pathingState = 0;
                tmpTarget = transform.position;
            }
            else if (distanceToPlayer < sleepAIDistance) // Pathing AI
            {
                pathingState = 1;
            }
            else // Sleep
            {
                pathingState = 2;
            }
            pathfindingDelay = 0;
            path.Clear();
        } else
        {
            switch(pathingState)
            {
                case 0:
                    CombatAI();
                    break;
                case 1:
                    PathfindingAI();
                    break;
                case 2:
                    break;
            }
        }
        pathfindingDelay = pathfindingDelay + 1;
        float speed = rb.velocity.magnitude;
        if (speed != 0.0f)
        {
            //set velocity to decreased speed value
            rb.velocity = rb.velocity * (speed - 10f * Time.fixedDeltaTime) / speed;
        }
    }

    private void CombatAI()
    {
        if ( Mathf.Abs(transform.position.x - tmpTarget.x) < 1 || Mathf.Abs(transform.position.y - tmpTarget.y) < 1){
            target = true;
        }

        if(target == true){
            tmpTarget = new Vector3(transform.position.x + Random.Range(-2f, 2), transform.position.y + Random.Range(-2f, 2), 0);
            target = false;
        }

        if(target == false){
            transform.position = Vector3.Lerp(transform.position, tmpTarget, 0.05f);
        }
    }

    private void PathfindingAI()
    {
        if (path == null || path.Count == 0)
        {
            path = gameObject.GetComponent<EnemyPathfinding>().PathFinding(transform.position, player.transform.position);
            if (path != null || path.Count > 0)
            {
                pathfindingTarget = path.Pop();
            } else
            {
                pathingState = 2;
            }
        }else if (Mathf.Abs(pathfindingTarget.x - transform.position.x) < 1 && Mathf.Abs(pathfindingTarget.y - transform.position.y) < 1)
        {
            pathfindingTarget = path.Pop();
        }
        transform.position = Vector3.Lerp(transform.position, pathfindingTarget, 0.05f);

    }

    public void CombatStats(int level)
    {
        if (level > 4)
        {
            level = 4;
        }
        MaxHitPoints = healthValues[level];
        HitPoints = healthValues[level];
        gameObject.GetComponent<SpriteRenderer>().color = colorValues[level];
        gameObject.GetComponent<EnemyTargetting>().ChangeScalingVars(attackRangeValues[level], damageValues[level],
            attackSpeedValues[level], projectileSpeed[level]);
        speed = speedValues[level];
    }
}
