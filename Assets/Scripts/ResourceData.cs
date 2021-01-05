using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResourceData
{
    // IDs of each resource
    public const int NULL_RESOURCE = 0;

    // sprites that each resource uses
    // each "path/to/resource" is RELATIVE TO RESOURCES FOLDER
    public static Dictionary<int, Sprite> resourceSprites = new Dictionary<int, Sprite>
    {
        { NULL_RESOURCE, null },
        // { RESOURCE_ID, Resources.Load<Sprite>("path/to/resource") }
    };
    // string representations of each resource
    public static Dictionary<int, string> resourceNames = new Dictionary<int, string>
    {
        { NULL_RESOURCE, "<NULL>" },
    };

    // int ID of this resource
    [SerializeField]
    public int resourceID;
    // amount of resource referenced by this data
    [SerializeField]
    public int count;
}