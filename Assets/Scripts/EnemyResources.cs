using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyResources : MonoBehaviour
{
    public GameObject EnemyShip;
    public void CreateResource() {
        Debug.Log(EnemyShip.transform.position);
        
    }
}
