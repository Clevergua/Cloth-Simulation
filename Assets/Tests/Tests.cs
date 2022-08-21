using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Tests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void 创建布料测试()
        {
            var mesh = new Mesh();
            var length = 3;
            var width = 3;
            var space = new Vector2(0.1f, 0.1f);
            var vertices = new List<Vector3>(length * width);
            var triangleCount = (width - 1) * (length - 1) * 2;
            var triangles = new List<int>(triangleCount * 3);
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    vertices.Add(new Vector3(x * space.x, 0, y * space.y));
                }
            }
            for (int y = 0; y < width - 1; y++)
            {
                for (int x = 0; x < length - 1; x++)
                {
                    var s = x + y * length;
                    triangles.Add(s);
                    triangles.Add(s + length);
                    triangles.Add(s + length + 1);

                    triangles.Add(s + length + 1);
                    triangles.Add(s + 1);
                    triangles.Add(s);

                }
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            // var cloth = new ClothSimulation.Cloth(mesh, ClothSimulation.ClothCalculationType.ExplicitEuler);
            // cloth.Update(0.01f);
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
