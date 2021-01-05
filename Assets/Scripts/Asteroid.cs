using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, TakesDamage
{
    public const float MIN_RADIUS = 0.2f;
    public const float DECELERATION_RATE = 0.1f;

    // movement vector
    private Vector2 direction;

    // Asteroid's RigidBody component
    private Rigidbody2D myRigidbody;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(Vector2 v)
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        direction = v;

        myRigidbody.AddForce(direction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void TakeDamage(float damage)
    {
        System.Random random = new System.Random();
        // create two child asteroids
        float radius = transform.localScale.x * GetComponent<CircleCollider2D>().radius;
        float childRadius = radius / 1.5f;
        transform.transform.localScale *= childRadius / radius; // reduce radius so that children can be cloned from this object

        Vector2 split = transform.right.normalized; // rotation of asteroids should be random, so we choose arbitrary vector for children
        Vector2[] positions = { (Vector2)transform.position + split * childRadius, (Vector2)transform.position - split * childRadius };

        if (childRadius >= MIN_RADIUS) // if children are too small, they aren't generated
        {
            GameObject[] children =
            {
                Instantiate(gameObject, positions[0], Quaternion.AngleAxis((float)random.NextDouble() * 360.0f, Vector3.forward)),
                Instantiate(gameObject, positions[1], Quaternion.AngleAxis((float)random.NextDouble() * 360.0f, Vector3.forward))
            };
            children[0].GetComponent<Asteroid>().Initialize(myRigidbody.velocity + split * 40.0f);
            children[1].GetComponent<Asteroid>().Initialize(myRigidbody.velocity - split * 40.0f);
        }

        Destroy(gameObject);
    }

    // most other objects destroy asteroids on collision
    public void OnCollisionEnter2D(Collision2D collision)
    {
        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        if (!(collider is null))
        {
            Asteroid asteroid = collision.collider.gameObject.GetComponent<Asteroid>();
            if (asteroid is null) // asteroids don't take damage from asteroids
                collider.TakeDamage(4.0f * transform.localScale.x); // smaller asteroids do less damage
        }
    }
}
