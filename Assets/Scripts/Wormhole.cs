using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// wormholes transport the player between levels
public class Wormhole : MonoBehaviour
{
    // true if this wormhole should teleport the player somewhere
    [SerializeField]
    private bool travel;

    public bool GetTravel() { return travel; }

    [SerializeField]
    private float rotationSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(bool b)
    {
        travel = b;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed);
    }

    // wormholes don't really change when they collide with something,
    // although maybe they could do an effect in the future
    public void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
