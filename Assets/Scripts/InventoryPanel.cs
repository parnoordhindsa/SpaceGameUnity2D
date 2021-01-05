using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    // text counting number of items in gui
    private TextMeshProUGUI itemCount;
    // name of item this panel displays
    [SerializeField]
    private string itemName;
    // index of our item in GameManager
    private int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCount()
    {
        GameManager inv = GameManager.instance;

        // try to find index if not already set
        if (index == -1)
        {
            for (int i = 0; i < inv.items.Count; ++i)
            {
                if (inv.items[i].itemName == itemName)
                {
                    index = i;
                    break;
                }
            }
        }

        int count;
        if (index == -1)
            count = 0;
        else
            count = inv.itemsNumber[index];

        if (itemCount is null)
            itemCount = transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        itemCount.text = Convert.ToString(count);
    }
}
