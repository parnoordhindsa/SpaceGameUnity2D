using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }

    public PathfindingNode Parent { get; set; }
    public Vector3Int Position { get; set; }
    public PathfindingNode(Vector3Int pos)
    {
        this.Position = pos;
    }

}
