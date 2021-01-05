using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour, TakesDamage
{
    // stored since it's used multiple times
    private LootTable<GameObject> lootTable;
    // number of times shot by player recently and resource was dropped
    private int damageTaken;
    // most damage planet can take before dropping no more resources
    public const int MAX_DAMAGE = 20;
    // stored random generator
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        lootTable = GameObject.FindWithTag("Level").GetComponent<Level>().PlanetDrops;
        damageTaken = 0;
        random = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // a planet should basically survive any collision
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // if player shot planet, maybe drop resource
        PlayerFireRight playerShot = collision.collider.gameObject.GetComponent<PlayerFireRight>();
        if (!(playerShot is null))
        {
            // resource drops become more rare each time damage is taken
            int prob = random.Next() % MAX_DAMAGE;
            if (prob >= damageTaken)
            {
                // spawn resource
                ++damageTaken;
                GameObject resourcePrefab = lootTable.Get();
                GameObject resource = Instantiate(resourcePrefab, collision.collider.transform.position, Quaternion.identity);

                // give the item a little push
                Vector2 direction = resource.transform.position - transform.position;
                direction.Normalize();
                resource.GetComponent<Rigidbody2D>().AddForce(direction * 20.0f);
            }
            return; // player laser does not take damage so return early
        }

        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        if (!(collider is null))
        {
            collider.TakeDamage(4.0f);
        }
    }

    public void TakeDamage(float damage)
    {
        
    }
}
