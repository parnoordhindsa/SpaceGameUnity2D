using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCounts()
    {
        for (int i = 1; i <= 4; ++i)
        {
            transform.GetChild(i).gameObject.GetComponent<InventoryPanel>().UpdateCount();
        }
    }
}
