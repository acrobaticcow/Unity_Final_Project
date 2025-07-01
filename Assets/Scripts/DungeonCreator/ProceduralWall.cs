using UnityEngine;

public static class ProceduralWall
{
    public static Mesh GenerateMesh(Vector3 size)
    {
        var mesh = new Mesh();
        var width = size.x;
        var height = size.y;
        var thickness = size.z;

        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[36];

        // Define vertices
        vertices[0] = new Vector3(-width / 2, -height / 2, -thickness / 2);
        vertices[1] = new Vector3(width / 2, -height / 2, -thickness / 2);
        vertices[2] = new Vector3(width / 2, height / 2, -thickness / 2);
        vertices[3] = new Vector3(-width / 2, height / 2, -thickness / 2);

        vertices[4] = new Vector3(-width / 2, -height / 2, thickness / 2);
        vertices[5] = new Vector3(width / 2, -height / 2, thickness / 2);
        vertices[6] = new Vector3(width / 2, height / 2, thickness / 2);
        vertices[7] = new Vector3(-width / 2, height / 2, thickness / 2);

        // Define triangles
        // Front face
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        // Back face
        triangles[6] = 4;
        triangles[7] = 5;
        triangles[8] = 6;
        triangles[9] = 4;
        triangles[10] = 6;
        triangles[11] = 7;

        // Left face
        triangles[12] = 0;
        triangles[13] = 7;
        triangles[14] = 3;
        triangles[15] = 0;
        triangles[16] = 4;
        triangles[17] = 7;

        // Right face
        triangles[18] = 1;
        triangles[19] = 2;
        triangles[20] = 6;
        triangles[21] = 1;
        triangles[22] = 6;
        triangles[23] = 5;

        // Top face
        triangles[24] = 2;
        triangles[25] = 3;
        triangles[26] = 7;
        triangles[27] = 2;
        triangles[28] = 7;
        triangles[29] = 6;

        // Bottom face
        triangles[30] = 0;
        triangles[31] = 1;
        triangles[32] = 5;
        triangles[33] = 0;
        triangles[34] = 5;
        triangles[35] = 4;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
