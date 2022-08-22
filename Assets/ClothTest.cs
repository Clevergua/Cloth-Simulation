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
    private int[] triangles;
    [SerializeField]
    private float deltaTime;
    [SerializeField]
    private List<FixedParticle> fixedParticleList;
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
    [SerializeField]
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            var length = 16;
            var width = 16;
            var space = new Vector2(1f, 1f);
            var vertices = new List<Vector3>(length * width);
            var triangleCount = (width - 1) * (length - 1) * 2;
            triangles = new int[triangleCount * 3];
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    vertices.Add(new Vector3(x * space.x, Mathf.Sqrt(x * space.x * x * space.x + y * space.y * y * space.y), y * space.y));
                }
            }
            var index = 0;
            for (int y = 0; y < width - 1; y++)
            {
                for (int x = 0; x < length - 1; x++)
                {
                    var s = x + y * length;
                    triangles[index++] = (s + length + 1);
                    triangles[index++] = (s + length);
                    triangles[index++] = (s);
                    triangles[index++] = (s);
                    triangles[index++] = (s + 1);
                    triangles[index++] = (s + length + 1);
                }
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
        }
        else
        {
            //do not create mesh
            var oldMesh = mesh;
            mesh = new Mesh();
            mesh.SetVertices(oldMesh.vertices);
            mesh.SetTriangles(oldMesh.triangles, 0);
            triangles = oldMesh.triangles;
        }
        Dictionary<int, Vector3> index2FixedPosition = new Dictionary<int, Vector3>();
        foreach (var fixedParticle in fixedParticleList)
        {
            index2FixedPosition[fixedParticle.index] = fixedParticle.position;
        }
        cloth = new ClothSimulation.Cloth(mesh, clothCalculationType, fixedParticleList, g, springK, damping, particleMass, bendingK);
        meshFilter.mesh = mesh;
    }
    void Update()
    {
        UpdateOnce();
    }

    // Update is called once per frame
    public void UpdateOnce()
    {
        if (cloth != null)
        {
            var verticesCache = cloth.Update(deltaTime);
            meshFilter.mesh.SetVertices(verticesCache);
            meshFilter.mesh.SetTriangles(triangles, 0);
            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.OptimizeReorderVertexBuffer();
        }
    }
}
