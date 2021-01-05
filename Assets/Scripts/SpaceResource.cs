using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceResource : MonoBehaviour
{
    // resource data of object
    private ResourceData resource;

    // unity testing variables - should be removed when resources are fully implemented
    [SerializeField]
    int resourceID = 0;
    [SerializeField]
    int count = 1;

    // sets gameobject to render associated resource sprite
    public void Initialize(ResourceData resourceData)
    {
        resource = resourceData;
        gameObject.GetComponent<SpriteRenderer>().sprite = ResourceData.resourceSprites[resourceData.resourceID];
    }

    // Start is called before the first frame update
    void Start()
    {
        // unity testing - generates ResourceData struct and calls Initialize
        resource = new ResourceData { resourceID = resourceID, count = count };
        Initialize(resource);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: have player pick up resource
    }
}
