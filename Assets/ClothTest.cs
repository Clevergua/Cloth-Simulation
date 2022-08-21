using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClothSimulation;
public class ClothTest : MonoBehaviour
{
    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    ClothCalculationType clothCalculationType;
    ClothSimulation.Cloth cloth;
    private List<int> triangles;
    [SerializeField]
    private float deltaTime;
    [SerializeField]
    private List<int> movableIndices;
    [SerializeField]
    private Vector3 g;
    [SerializeField]
    private float springK;
    [SerializeField]
    private float damping;
    [SerializeField]
    private float particleMass;
    [SerializeField]
    private float bendingK;

    // Start is called before the first frame update
    void Start()
    {
        var mesh = new Mesh();
        var length = 16;
        var width = 16;
        var space = new Vector2(1f, 1f);
        var vertices = new List<Vector3>(length * width);
        var triangleCount = (width - 1) * (length - 1) * 2;
        triangles = new List<int>(triangleCount * 3);
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                vertices.Add(new Vector3(x * space.x, Mathf.Sqrt(x * space.x * x * space.x + y * space.y * y * space.y), y * space.y));
            }
        }
        for (int y = 0; y < width - 1; y++)
        {
            for (int x = 0; x < length - 1; x++)
            {
                var s = x + y * length;
                triangles.Add(s + length + 1);
                triangles.Add(s + length);
                triangles.Add(s);

                triangles.Add(s);
                triangles.Add(s + 1);
                triangles.Add(s + length + 1);
            }
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        HashSet<int> movableIndexSet = new HashSet<int>();
        foreach (var index in movableIndices)
        {
            if (!movableIndexSet.Contains(index))
            {
                movableIndexSet.Add(index);
            }
            else
            {
                //do nothing..
            }
        }
        cloth = new ClothSimulation.Cloth(mesh, clothCalculationType, movableIndexSet, g, springK, damping, particleMass, bendingK);
        meshFilter.mesh = mesh;
    }
    void Update()
    {
        UpdateOnce();
    }

    // Update is called once per frame
    public void UpdateOnce()
    {
        var verticesCache = cloth.Update(deltaTime);
        meshFilter.mesh.SetVertices(verticesCache);
        meshFilter.mesh.SetTriangles(triangles, 0);
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.OptimizeReorderVertexBuffer();
    }
}
