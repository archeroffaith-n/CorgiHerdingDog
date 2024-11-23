using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGeneration : MonoBehaviour
{
    public int xNumVert = 20;
    public int zNumVert = 20;
    public float xSize = 20.0f;
    public float zSize = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        GenerateMesh(mesh);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        transform.position = new Vector3(-xSize / 2, 0 , -zSize / 2);
    }

    void GenerateMesh(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[(xNumVert + 1) * (zNumVert + 1)];

        int currentIndex = 0;
        float y = 0.0f;
        for (int x = 0; x < xNumVert + 1; x++) {
            for (int z = 0; z < zNumVert + 1; z++) {
                y = GenerateHeight(x * 1.0f / xNumVert * xSize, z * 1.0f / zNumVert * zSize);
                vertices[currentIndex] = new Vector3(x * 1.0f / xNumVert * xSize, y, z * 1.0f / zNumVert * zSize);
                currentIndex += 1;
            }
        }

        int[] triangles = new int[xNumVert * zNumVert * 6];
        int currentVertex = 0;
        currentIndex = 0;
        for (int x = 0; x < xNumVert; x++) {
            for (int z = 0; z < zNumVert; z++) {
                triangles[currentIndex++] = currentVertex + 0;
                triangles[currentIndex++] = currentVertex + 1;
                triangles[currentIndex++] = currentVertex + 2 + zNumVert;
                triangles[currentIndex++] = currentVertex + 0;
                triangles[currentIndex++] = currentVertex + 2 + zNumVert;
                triangles[currentIndex++] = currentVertex + 1 + zNumVert;
                currentVertex++;
            }
            currentVertex++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    float GenerateHeight(float x, float z)
    {
        return Mathf.PerlinNoise(x * 0.03f, z * 0.03f) * 10.0f;
    }
}
