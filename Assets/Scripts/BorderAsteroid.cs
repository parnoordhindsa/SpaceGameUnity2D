using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderAsteroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        
    }

    // most other objects destroy asteroids on collision
    public void OnCollisionEnter2D(Collision2D collision)
    {
        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        if (!(collider is null))
        {
            collider.TakeDamage(10.0f);
        }
    }
}
