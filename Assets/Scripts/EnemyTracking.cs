using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracking : MonoBehaviour
{
    // TODO: MAKE IT WORK

    private GameObject player;
    public float enemyTrackingRange;
    private float distanceToPlayer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void FixedUpdate()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer >= enemyTrackingRange)
        {
            return;
        }
    }
}
