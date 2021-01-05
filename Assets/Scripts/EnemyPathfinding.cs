using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System.Linq;

public class EnemyPathfinding : MonoBehaviour
{
    private Vector3Int startPos, endPos;
    private PathfindingNode currentNode;
    private HashSet<PathfindingNode> openList;
    private HashSet<PathfindingNode> closedList;
    private Stack<Vector3> path;

    private Dictionary<Vector3Int, PathfindingNode> allNodes = new Dictionary<Vector3Int, PathfindingNode>();

    public void Initialize()
    {
        currentNode = GetNode(startPos);

        openList = new HashSet<PathfindingNode>();
        closedList = new HashSet<PathfindingNode>();

        openList.Add(currentNode);
    }

    public void ResetVars()
    {
        currentNode = null;
        openList.Clear();
        closedList.Clear();
        path.Clear();
        allNodes.Clear();
    }

    public Stack<Vector3> PathFinding(Vector3 start, Vector3 end)
    {
        startPos = VectorFloatToInt(start);
        endPos = VectorFloatToInt(end);

        if (currentNode != null)
        {
            ResetVars();
        }

        Initialize();

        while(openList.Count > 0 && (path == null || path.Count == 0))
        {
            List<PathfindingNode> neighbours = FindNeighbours(currentNode.Position);

            ExamineNeighbours(neighbours, currentNode);

            UpdateCurrentNode(ref currentNode);

            path = GeneratePath(currentNode);
        }

        return path;

    }
    /*
     * GetNode checks if the given position is already a node, else it creates the node and adds it
     * Returns the node regardless
     */
    public PathfindingNode GetNode(Vector3Int pos)
    {
        if(allNodes.ContainsKey(pos))
        {
            return allNodes[pos];
        } else
        {
            PathfindingNode node = new PathfindingNode(pos);
            allNodes.Add(pos, node);
            return node;
        }
    }
    /*
     * FindNeighbours grabs all valid nearby nodes in a list and returns that list
     * 
     */
    private List<PathfindingNode> FindNeighbours(Vector3Int parentPos)
    {
        List<PathfindingNode> neighbours = new List<PathfindingNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbourPos = new Vector3Int(parentPos.x - x, parentPos.y - y, parentPos.z);
                
                if (x != 0 || y != 0)
                {
                    /*if (x != 1) {
                        if (Physics2D.OverlapPoint(new Vector2(neighbourPos.x - 0.5f, neighbourPos.y)) != null)
                        {
                            hitObject = true;
                        }
                    }
                    if (x != -1)
                    {
                        if(Physics2D.OverlapPoint(new Vector2(neighbourPos.x + 0.5f, neighbourPos.y)) != null)
                        {
                            hitObject = true;
                        }
                    }
                    if (y != 1) 
                    {
                        if(Physics2D.OverlapPoint(new Vector2(neighbourPos.x, neighbourPos.y + 0.5f)) != null)
                        {
                            hitObject = true;
                        }
                    }
                    if (y != -1)
                    {
                        if(Physics2D.OverlapPoint(new Vector2(neighbourPos.x, neighbourPos.y - 0.5f)) != null)
                        {
                            hitObject = true;
                        }
                    }*/

                    //Collider2D hit = Physics2D.OverlapPoint(new Vector2(neighbourPos.x, neighbourPos.y));
                    Collider2D newHit = Physics2D.OverlapCircle(new Vector2(neighbourPos.x + x * 0.1f, neighbourPos.y + y * 0.1f), 0.2f);
                    if (neighbourPos != startPos && newHit == null)
                    {
                        PathfindingNode neighbour = GetNode(neighbourPos);
                        neighbours.Add(neighbour);
                    }
                }
            }
        }
        return neighbours;
    }

    private void ExamineNeighbours(List<PathfindingNode> neighbours, PathfindingNode current)
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            PathfindingNode neighbour = neighbours[i];
            int gScore = CalculateGScore(neighbours[i].Position, current.Position);

            if (openList.Contains(neighbour))
            {
                if(current.G + gScore < neighbour.G)
                {
                    CalculateValues(current, neighbour, gScore);
                }
            } else if (!closedList.Contains(neighbour))
            {
                CalculateValues(current, neighbour, gScore);
                openList.Add(neighbour);
            }
        }
    }

    private int CalculateGScore(Vector3Int neighbour, Vector3Int current)
    {
        int gScore = 0;
        int x = current.x - neighbour.x;
        int y = current.y - neighbour.y;
        if (Mathf.Abs(x - y) % 2 == 1)
        {
            gScore = 10;
        } else
        {
            gScore = 14;
        }
        return gScore;
    }

    private void CalculateValues(PathfindingNode parent, PathfindingNode neighbour, int cost)
    {
        neighbour.Parent = parent;
        neighbour.G = parent.G + cost;
        neighbour.H = ((Mathf.Abs(neighbour.Position.x - endPos.x) + Mathf.Abs(neighbour.Position.y - endPos.y))) * 10;
        neighbour.F = neighbour.G + neighbour.H;
    }

    private void UpdateCurrentNode(ref PathfindingNode current)
    {
        openList.Remove(current);
        closedList.Add(current);

        if(openList.Count > 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }

    private Stack<Vector3> GeneratePath(PathfindingNode current)
    {
        if (Mathf.Abs(current.Position.x - endPos.x) < 2 && Mathf.Abs(current.Position.y - endPos.y) < 2)
        {
            Stack<Vector3> finalPath = new Stack<Vector3>();

            while (current.Position != startPos)
            {
                finalPath.Push(current.Position);

                current = current.Parent;
            }
            return finalPath;
        }
        return null;
    }

    private Vector3Int VectorFloatToInt(Vector3 current)
    {
        return new Vector3Int(Mathf.FloorToInt(current.x), Mathf.FloorToInt(current.y),
            Mathf.FloorToInt(current.z));
    }

}
