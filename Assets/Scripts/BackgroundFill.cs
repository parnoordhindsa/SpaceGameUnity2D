using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFill : MonoBehaviour
{
    
    private Camera mainCamera;
    public GameObject background;
    public float radius;

    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();

        float objectWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        float objectHeight = background.GetComponent<SpriteRenderer>().bounds.size.y;

        radius = GameObject.FindWithTag("Level").GetComponent<Level>().Radius;
        // GameObject c = Instantiate(background) as GameObject; 

        for(float k=radius * -1; k<=radius+objectHeight; k=k+objectHeight) {
            for(float i=radius * -1; i<=radius+objectWidth; i = i+objectWidth){
                GameObject clone = Instantiate(background) as GameObject;
                clone.transform.position = new Vector3(i, k, 0);
            }
        }
    }
}
