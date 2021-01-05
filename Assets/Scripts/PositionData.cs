using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SpaceData
{
    Vector2 Pos { get; set; }
    float Radius { get; set; }
}

// manages a table of spatial bodies for faster position lookup
public class PositionData
{
    // how large of a size each entry in the table should cover
    private float chunkSize;
    // highest radius of a spatial body seen so far
    private float maxRadius;
    // table of spatial bodies
    private Dictionary<int, Dictionary<int, List<SpaceData>>> data;

    public PositionData(float s)
    {
        chunkSize = s;
        maxRadius = 0.0f;
        data = new Dictionary<int, Dictionary<int, List<SpaceData>>>();
    }

    // add a body to the table
    public void Add(SpaceData spaceData)
    {
        // transform position of body to appropriate indices in table
        int x = Mathf.FloorToInt(spaceData.Pos.x / chunkSize);
        int y = Mathf.FloorToInt(spaceData.Pos.y / chunkSize);

        if (spaceData.Radius > maxRadius)
            maxRadius = spaceData.Radius;

        if (!data.ContainsKey(x))
        {
            data.Add(x, new Dictionary<int, List<SpaceData>>());
            data[x].Add(y, new List<SpaceData>());
            data[x][y].Add(spaceData);
        }
        else if (!data[x].ContainsKey(y))
        {
            data[x].Add(y, new List<SpaceData>());
            data[x][y].Add(spaceData);
        }
        else
            data[x][y].Add(spaceData);
    }

    // return all spatial bodies in the neighbourhood of spaceData
    public List<SpaceData> GetNeighbours(SpaceData spaceData)
    {
        int x = Mathf.FloorToInt(spaceData.Pos.x / chunkSize);
        int y = Mathf.FloorToInt(spaceData.Pos.y / chunkSize);
        // radius large enough to find any possible colliding spatial body
        int r = 11; //Mathf.CeilToInt((spaceData.Radius + maxRadius) / chunkSize) + 1;

        List<SpaceData> result = new List<SpaceData>();
        for (int i = x - r; i <= x + r; ++i)
        {
            if (data.ContainsKey(i))
            {
                for (int j = y - r; j <= y + r; ++j)
                {
                    if (data[i].ContainsKey(j))
                    {
                        result.AddRange(data[i][j]);
                    }
                }
            }
        }
        return result;
    }
}
