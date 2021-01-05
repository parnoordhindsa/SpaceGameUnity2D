using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{

    public Transform target;
    public float smoothing;
    public float zoomSize = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize(GameObject player)
    {
        target = player.transform;
        smoothing = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Debug.Log("Greater than zero");
            if (zoomSize > 2)
                zoomSize -= 1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Debug.Log("LESS THAN ZERO");
            if (zoomSize < 10)
                zoomSize += 1;
        }
        GetComponent<Camera>().orthographicSize = zoomSize;
    }
}
