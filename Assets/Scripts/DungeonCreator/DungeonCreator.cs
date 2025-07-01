using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth,
        dungeonLength;
    public int roomWidthMin,
        roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material material;

    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;

    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;

    [Range(0, 2)]
    public int roomOffset;
    #region Wall Data
    public GameObject wallVertical,
        wallHorizontal;
    public Renderer wallHorizontalRenderer;
    public MeshFilter wallHorizontalMeshFilter;
    Vector2 wallSize;
    Mesh wallMesh;
    Material wallMaterial;
    readonly List<Matrix4x4> wallMatricesN = new();
    #endregion
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Node> nodes;

    // Start is called before the first frame update
    void Awake()
    {
        wallSize = wallHorizontalRenderer.bounds.size;
        wallMesh = wallHorizontalMeshFilter.sharedMesh;
        wallMaterial = wallHorizontalRenderer.sharedMaterial;
    }

    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();
        DugeonGenerator generator = new DugeonGenerator(dungeonWidth, dungeonLength);
        nodes = generator.CalculateDungeon(
            maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerMidifier,
            roomOffset,
            corridorWidth
        );
        // GameObject wallParent = new GameObject("WallParent");
        // wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();
        foreach (Node node in nodes)
        {
            CreateMesh(node.BottomLeftAreaCorner, node.TopRightAreaCorner);
            // CreateWallsTest(room.Size);
        }
        // CreateWalls(wallParent);
        CreatesWallTest2();
    }

    // TODO more precisely fit the wall
    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    /// <summary>
    /// Creates walls for a room based on the specified room size.
    /// The number of walls is determined by the width of the room and the width of the wall.
    /// Each wall is positioned and scaled accordingly to fit within the room dimensions.
    /// </summary>
    /// <param name="roomSize">The size of the room, represented as a Vector2 where x is the width and y is the height.</param>
    private void CreateWallsTest(Vector2 roomSize)
    {
        int wallCount = Mathf.Max(1, (int)(roomSize.x / wallSize.x));
        float scale = roomSize.x / wallCount / wallSize.x;
        for (int i = 0; i < wallCount; i++)
        {
            var t =
                transform.position
                + new Vector3(
                    -roomSize.x / 2 + wallSize.x * scale / 2 + i * scale * wallSize.x,
                    roomSize.y / 2,
                    0
                );
            var r = transform.rotation;
            var s = new Vector3(scale, 1, 1);
            var mat = Matrix4x4.TRS(t, r, s);
            wallMatricesN.Add(mat);
        }
    }

    private void CreatesWallTest2()
    {
        GameObject wallParent = new("WallParent");
        wallParent.transform.parent = transform;
        foreach (Node node in nodes)
        {
            if (node is CorridorNode corridorNode)
            {
                switch (corridorNode.Structure1WallFaceThatConnectToCorridor)
                {
                    case CorridorNode.WallFace.Up:
                        float bottomLeftCorridorWallLength =
                            corridorNode.BottomLeftAreaCorner.x
                            - corridorNode.structure1.BottomLeftAreaCorner.x;
                        Vector3 anchorPosition =
                            new(
                                (
                                    corridorNode.BottomLeftAreaCorner.x
                                    + corridorNode.structure1.BottomLeftAreaCorner.x
                                ) / 2,
                                0,
                                corridorNode.BottomLeftAreaCorner.y
                            );
                        GameObject wallChild = new("Wall");
                        wallChild.transform.parent = wallParent.transform;
                        wallChild.transform.position = anchorPosition;
                        Mesh mesh = ProceduralWall.GenerateMesh(
                            new(bottomLeftCorridorWallLength, 10, 2)
                        );
                        wallChild.AddComponent<MeshRenderer>();
                        wallChild.AddComponent<MeshFilter>().mesh = mesh;
                        wallChild.AddComponent<MeshCollider>();

                        Vector3 anchorPosition2 =
                            new(
                                (
                                    corridorNode.TopRightAreaCorner.x
                                    + corridorNode.structure1.TopRightAreaCorner.x
                                ) / 2,
                                0,
                                corridorNode.structure1.TopRightAreaCorner.y
                            );
                        break;
                    case CorridorNode.WallFace.Down:
                        break;
                    case CorridorNode.WallFace.Right:
                        break;
                    case CorridorNode.WallFace.Left:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void RenderWalls()
    {
        if (wallMatricesN != null)
        {
            Graphics.DrawMeshInstanced(
                wallMesh,
                0,
                wallMaterial,
                wallMatricesN.ToArray(),
                wallMatricesN.Count
            );
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        // TODO remove hard-code
        Instantiate(
            wallPrefab,
            wallPosition + new Vector3(-0.5f, 5, -0.5f), // shift the anchor point to the bottom of the wall and outside the the edge of the floor
            Quaternion.identity,
            wallParent.transform
        );
    }

    void OnDrawGizmos()
    {
        foreach (var node in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new(node.BottomLeftAreaCorner.x, 0, node.BottomLeftAreaCorner.y), 1);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new(node.TopRightAreaCorner.x, 0, node.TopRightAreaCorner.y), 1);
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
        Mesh mesh = new();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor =
            new("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition
            );
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition
            );
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition
            );
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition
            );
        }
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

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
