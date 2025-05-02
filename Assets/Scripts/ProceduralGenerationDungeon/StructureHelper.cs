using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeafs(Node parentNode)
    {
        Queue<Node> nodesToCheck = new();
        List<Node> lowestLeafs = new();
        // * guard
        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new() { parentNode };
        }
        // * populate nodesToCheck with the closest node to parent
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();
            // * add the lowest leaf if possible
            if (currentNode.ChildrenNodeList.Count == 0)
            {
                lowestLeafs.Add(currentNode);
            }
            // * add current node's child node to nodesToCheck
            else
            {
                foreach (var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }
        return lowestLeafs;
    }

    /// <summary>
    /// This method calculate the coordinate the corner of a randomly size room
    /// </summary>
    /// <param name="boundaryBottomLeftPoint"></param>
    /// <param name="boundaryTopRightPoint"></param>
    /// <param name="pointModifier">[0,1] range use to scale the size of the room</param>
    /// <param name="offset">Could used to reserve some space for walls.</param>
    /// <returns>Coordinate of the corner</returns>
    public static Vector2Int GenerateBottomLeftCornerBetween(
        Vector2Int boundaryBottomLeftPoint,
        Vector2Int boundaryTopRightPoint,
        float pointModifier,
        int offset
    )
    {
        int minX = boundaryBottomLeftPoint.x + offset;
        int maxX = boundaryTopRightPoint.x - offset;
        int minY = boundaryBottomLeftPoint.y + offset;
        int maxY = boundaryTopRightPoint.y - offset;
        return new(
            // * pick a value between the modified distance of min/max X
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            // * pick a value between the modified distance of min/max Y
            Random.Range(minY, (int)(minY + (maxY - minY) * pointModifier))
        );
    }

    /// <summary>
    /// This method calculate the coordinate the corner of a randomly size room
    /// </summary>
    /// <param name="boundaryLeftPoint"></param>
    /// <param name="boundaryRightPoint"></param>
    /// <param name="pointModifier">[0,1] range use to scale the size of the room</param>
    /// <param name="offset">Could used to reserve some space for walls.</param>
    /// <returns>Coordinate of the corner</returns>
    public static Vector2Int GenerateTopRightCornerBetween(
        Vector2Int boundaryBottomLeftPoint,
        Vector2Int boundaryTopRightPoint,
        float pointModifier,
        int offset
    )
    {
        int minX = boundaryBottomLeftPoint.x + offset;
        int maxX = boundaryTopRightPoint.x - offset;
        int minY = boundaryBottomLeftPoint.y + offset;
        int maxY = boundaryTopRightPoint.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY)
        );
    }
}
