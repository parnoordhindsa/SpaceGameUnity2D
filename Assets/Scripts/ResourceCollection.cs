using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollection : MonoBehaviour

{
    [SerializeField]
    private float magneticRange;

    [SerializeField]
    private float magneticForce;


    // Update is called once per frame
    void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, magneticRange);
        foreach (Collider2D cd in hitColliders ?? System.Linq.Enumerable.Empty<Collider2D>())
        {
            if (cd.CompareTag("Resource"))
            {
                //Finds the distance to the object, uses that to scale how much force is added to the resource
                float distance = Vector2.Distance(transform.position, cd.transform.position);
                cd.GetComponent<Rigidbody2D>().AddForce((transform.position - cd.transform.position)*(magneticForce)*(1/((distance+0.01f)*(distance))));
            }
        }
    }


}
