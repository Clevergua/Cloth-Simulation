namespace ClothSimulation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Cloth
    {
        private List<Particle> particles;
        private Vector3 g = new Vector3(0, -10, 0);
        private float springK = 1000;
        private float damping = 0.9f;
        private float particleMass = 1;
        private float bendingK = 5;
        private List<FixedParticle> fixedParticleList;
        private List<Spring> springs;
        private Vector3[] particleIndex2Gradient;
        private Vector3[] xHats;
        private Mesh mesh;
        private Vector3[] verticesResult;
        private ClothCalculationType clothCalculationType;
        private List<BendContraintion> bendConstraintions;

        public Cloth(Mesh mesh, ClothCalculationType clothCalculationType, List<FixedParticle> fixedParticleList, Vector3 g, float springK, float damping, float particleMass, float bendingK)
        {
            this.g = g;
            this.springK = springK;
            this.damping = damping;
            this.particleMass = particleMass;
            this.bendingK = bendingK;
            this.fixedParticleList = fixedParticleList;
            this.mesh = mesh;
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            var triangleStride = 3;
            var triangleCount = triangles.Length / triangleStride;
            //构建TripleList
            var tripleList = new List<TriangleEdgeTriple>(triangles.Length);
            for (int triangeleIndex = 0; triangeleIndex < triangleCount; triangeleIndex++)
            {
                var triangleStartIndex = triangeleIndex * triangleStride;
                tripleList.Add(new TriangleEdgeTriple(triangles[triangleStartIndex + 0], triangles[triangleStartIndex + 1], triangeleIndex));
                tripleList.Add(new TriangleEdgeTriple(triangles[triangleStartIndex + 0], triangles[triangleStartIndex + 2], triangeleIndex));
                tripleList.Add(new TriangleEdgeTriple(triangles[triangleStartIndex + 1], triangles[triangleStartIndex + 2], triangeleIndex));
            }
            tripleList.Sort();
            var tripleCache = tripleList[0];

            var edgeList = new List<Edge>() { new Edge(tripleCache.vertexIndex0, tripleCache.vertexIndex1) };
            bendConstraintions = new List<BendContraintion>();
            var index = 1;
            while (index < tripleList.Count)
            {
                var current = tripleList[index];
                if (current.vertexIndex0 == tripleCache.vertexIndex0 && current.vertexIndex1 == tripleCache.vertexIndex1)
                {
                    int bvi0 = -1;
                    int bvi1 = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        var cacheVertexIndex = triangles[tripleCache.triangeleIndex * 3 + i];
                        if (cacheVertexIndex != tripleCache.vertexIndex0 && cacheVertexIndex != tripleCache.vertexIndex1)
                        {
                            bvi0 = cacheVertexIndex;
                        }
                        var currentVertexIndex = triangles[current.triangeleIndex * 3 + i];
                        if (currentVertexIndex != tripleCache.vertexIndex0 && currentVertexIndex != tripleCache.vertexIndex1)
                        {
                            bvi1 = currentVertexIndex;
                        }
                    }
                    var p0 = vertices[current.vertexIndex0];
                    var p1 = vertices[current.vertexIndex1] - p0;
                    var p2 = vertices[bvi0] - p0;
                    var p3 = vertices[bvi1] - p0;

                    var n1 = Vector3.Cross(p1, p2).normalized;
                    var n2 = Vector3.Cross(p1, p3).normalized;
                    var rest = Mathf.Acos(Vector3.Dot(n1, n2));
                    var bendConstraintion = new BendContraintion(current.vertexIndex0, current.vertexIndex1, bvi0, bvi1, rest);
                    bendConstraintions.Add(bendConstraintion);
                }
                else
                {
                    var edge = new Edge(current.vertexIndex0, current.vertexIndex1);
                    edgeList.Add(edge);
                    tripleCache = current;
                }
                index++;
            }

            this.particles = new List<Particle>(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                var movable = true;
                foreach (var item in fixedParticleList)
                {
                    if (item.index == i)
                    {
                        movable = false;
                        break;
                    }
                }
                particles.Add(new Particle(i, vertices[i], Vector3.zero, movable));
            }
            this.springs = new List<Spring>(edgeList.Count);
            // var edgeLengthList = new List<float>();
            foreach (var edge in edgeList)
            {
                var v0 = vertices[edge.vertexIndex0];
                var v1 = vertices[edge.vertexIndex1];
                springs.Add(new Spring(edge.vertexIndex0, edge.vertexIndex1, (v1 - v0).magnitude));
            }
            particleIndex2Gradient = new Vector3[particles.Count];
            xHats = new Vector3[particles.Count];
            verticesResult = new Vector3[vertices.Length];

            this.clothCalculationType = clothCalculationType;
        }


        public Vector3[] Update(float deltaTime)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                var particle = particles[index];
                if (!particle.movable)
                {
                    xHats[index] = particle.position;
                }
                else
                {
                    particle.velocity *= damping;
                    xHats[index] = particle.position + deltaTime * particle.velocity;
                    particle.position = xHats[index];
                }
            }
            if (clothCalculationType == ClothCalculationType.ExplicitEuler)
            {
                var particleIndex2Force = new Vector3[particles.Count];
                for (int index = 0; index < particles.Count; index++)
                {
                    var particle = particles[index];
                    particleIndex2Force[index] = Vector3.zero + particleMass * g;
                }
                foreach (var spring in springs)
                {

                    var v0 = particles[spring.particeleIndex0].position;
                    var v1 = particles[spring.particeleIndex1].position;
                    var dir = v0 - v1;
                    var force = (dir.magnitude / spring.length - 1) * springK * dir.normalized;
                    particleIndex2Force[spring.particeleIndex0] -= force;
                    particleIndex2Force[spring.particeleIndex1] += force;
                }
                for (int i = 0; i < particleIndex2Force.Length; i++)
                {
                    var particle = particles[i];
                    particle.velocity += (particleIndex2Force[i] / particleMass) * deltaTime;
                }
            }
            else
            {
                //newton function
                int times = 32;
                for (int i = 0; i < times; i++)
                {
                    for (int index = 0; index < particles.Count; index++)
                    {
                        var particle = particles[index];
                        particleIndex2Gradient[index] = (particle.position - xHats[index]) * particleMass / (deltaTime * deltaTime) - g * particleMass;
                    }
                    foreach (var spring in springs)
                    {
                        var v0 = particles[spring.particeleIndex0].position;
                        var v1 = particles[spring.particeleIndex1].position;
                        var dir = v0 - v1;
                        var deltaGradient = springK * (1 - spring.length / dir.magnitude) * dir;
                        particleIndex2Gradient[spring.particeleIndex0] += deltaGradient;
                        particleIndex2Gradient[spring.particeleIndex1] -= deltaGradient;
                    }
                    var areSmall = true;
                    for (int j = 0; j < particles.Count; j++)
                    {
                        var particle = particles[j];
                        if (!particle.movable)
                        {
                            xHats[j] = particle.position;
                        }
                        else
                        {
                            float quasiHessian = particleMass / (deltaTime * deltaTime) + 4 * springK;
                            var previousPosition = particle.position;
                            particle.position -= (1 / quasiHessian) * particleIndex2Gradient[j];
                            if ((particle.position - previousPosition).sqrMagnitude > 0.000001f)
                            {
                                areSmall = false;
                            }
                            else
                            {
                                //do nothing
                            }
                        }
                    }
                    if (areSmall)
                    {
                        //all deltaX are small.
                        break;
                    }
                    else
                    {
                        //do nothing
                    }
                }
            }

            //cal bending:
            for (int i = 0; i < bendConstraintions.Count; i++)
            {
                var constraintion = bendConstraintions[i];
                var x1 = particles[constraintion.vertexIndex0].position;
                var x2 = particles[constraintion.vertexIndex1].position;
                var x3 = particles[constraintion.vertexIndex2].position;
                var x4 = particles[constraintion.vertexIndex3].position;

                var n1 = Vector3.Cross(x1 - x3, x1 - x4);
                var n2 = Vector3.Cross(x2 - x4, x2 - x3);
                var e = x4 - x3;
                var u1 = e.magnitude * n1 / n1.sqrMagnitude;
                var u2 = e.magnitude * n2 / n2.sqrMagnitude;
                var u3 = (Vector3.Dot((x1 - x4), e) / e.magnitude) * n1 / n1.sqrMagnitude + Vector3.Dot(x2 - x4, e) / e.magnitude * n2 / n2.sqrMagnitude;
                var u4 = (Vector3.Dot((x1 - x3), e) / e.magnitude) * n1 / n1.sqrMagnitude + Vector3.Dot(x2 - x3, e) / e.magnitude * n2 / n2.sqrMagnitude;

                var p0 = x1;
                var p1 = x2 - p0;
                var p2 = x3 - p0;
                var p3 = x4 - p0;
                var nn1 = Vector3.Cross(p1, p2).normalized;
                var nn2 = Vector3.Cross(p1, p3).normalized;
                var currentRest = Mathf.Acos(Mathf.Clamp(Vector3.Dot(n1, n2), -1, 1));
                var fx = bendingK * e.sqrMagnitude / (n1.magnitude + n2.magnitude) * (Mathf.Sin(0.5f * (Mathf.PI - currentRest)) - Mathf.Sin(0.5f * (Mathf.PI - constraintion.rest)));

                var f1 = fx * u1;
                var f2 = fx * u2;
                var f3 = fx * u3;
                var f4 = fx * u4;

                particles[constraintion.vertexIndex0].velocity += (f1 / particleMass) * deltaTime;
                particles[constraintion.vertexIndex1].velocity += (f2 / particleMass) * deltaTime;
                particles[constraintion.vertexIndex2].velocity += (f3 / particleMass) * deltaTime;
                particles[constraintion.vertexIndex3].velocity += (f4 / particleMass) * deltaTime;
            }

            foreach (var item in fixedParticleList)
            {
                var particle = particles[item.index];
                particle.velocity = Vector3.zero;
                particle.position = item.position;
            }

            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                particle.velocity += (particle.position - xHats[i]) / deltaTime;
                verticesResult[i] = particle.position;
            }

            return verticesResult;
        }

    }
}

