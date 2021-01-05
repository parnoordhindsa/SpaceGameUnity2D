using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholePlanet : MonoBehaviour, TakesDamage
{
    public const float MAX_HEALTH = 50.0f;
    public int score;
    // planet health
    private float currentHealth;

    [SerializeField]
    GameObject wormholePrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MAX_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // destroy planet if health is 0
        if (currentHealth <= 0.0f)
        {

            Destroy(gameObject);

            score = PlayerPrefs.GetInt("score") + 20;
            Debug.Log(score);
            PlayerPrefs.SetInt("score", score);
    

            Wormhole wormhole = Instantiate(wormholePrefab, transform.position, Quaternion.identity).GetComponent<Wormhole>();
            wormhole.Initialize(true); // spawned wormhole should send player to next area
        }
    }
}
