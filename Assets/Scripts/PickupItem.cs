using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    
    public Item itemData;
    public GameObject pickupEffect;
    public GameObject pickupSound;

    // 
    private Vector2 destination;

    public void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player"){
            if (GameManager.instance.items.Count < GameManager.instance.slots.Length){
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
                GameObject soundEffect = Instantiate(pickupSound);
                Destroy(soundEffect, 0.8f);
                Destroy(gameObject);

                GameManager.instance.AddItem(itemData);
            }
            else {
                Debug.Log("You cannot pick any other item your bag is full");
            }
            
        }
    }
}
