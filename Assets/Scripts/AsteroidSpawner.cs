using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // asteroid prefab
    [SerializeField]
    private GameObject prefab;

    // asteroid spawning delay
    [SerializeField]
    private float delay_s = 2.0f;

    // asteroid lifetime
    [SerializeField]
    private float lifetime_s = 60.0f;

    // asteroid spawn "radius" around spawner
    [SerializeField]
    private float spawnRadius = 1.0f;

    // how fast the asteroids travel
    [SerializeField]
    private float spawnMagnitude = 50.0f;

    // asteroid mean size and maximum size variance, relative to default values
    [SerializeField]
    private float spawnSize = 0.5f;
    [SerializeField]
    private float spawnVar = 0.05f;

    // next spawn time
    private float nextSpawnTime = 0.0f;

    // cached random object
    private static System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            // generate random displacement for new asteroid
            Vector3 offset = transform.up * spawnRadius * Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble());
            offset.z = 0.0f; // ignore z value (should be zero anyway)
            GameObject asteroid = Instantiate(prefab, transform.position + offset, Quaternion.identity);
            // change asteroid size
            asteroid.transform.localScale *= spawnSize + spawnVar * Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble());
            // impart force to asteroid
            asteroid.GetComponent<Rigidbody2D>().AddForce(transform.right * spawnMagnitude);
            // kill object in lifetime_s seconds
            Destroy(asteroid, lifetime_s);
            nextSpawnTime = Time.time + delay_s;
        }
    }
}
