using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
   
    public float Speed = 9.5f;

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * Speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();

        if (!(collider is null))
        {
            collider.TakeDamage(1);
        }

        Destroy(gameObject);
    }
}
