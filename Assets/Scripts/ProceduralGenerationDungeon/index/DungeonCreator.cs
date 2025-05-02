using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth,
        dungeonLength;
    public int roomWidthMin,
        roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material Material;

    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;

    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;

    [Range(0, 2)]
    public int roomOffset;

    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        DungeonGenerator generator = new(dungeonWidth, dungeonLength);
        List<Node> rooms = generator.CalculateRooms(
            maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset
        );
        foreach (Node room in rooms)
        {
            CreateMesh(room.BottomLeftAreaCorner, room.TopRightAreaCorner);
        }
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[] { topLeftV, topRightV, bottomLeftV, bottomRightV };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        Mesh mesh =
            new()
            {
                vertices = vertices,
                uv = uvs,
                triangles = triangles
            };

        GameObject dungeonFloor =
            new("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = Material;
        dungeonFloor.transform.parent = transform;
    }

    private void Regenerate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        CreateDungeon();
    }

    private void AddWallPositionToList(
        Vector3 wallPosition,
        List<Vector3Int> wallList,
        List<Vector3Int> doorList
    )
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
}
