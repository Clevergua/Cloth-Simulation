using System;

namespace ClothSimulation
{
    internal struct TriangleEdgeTriple : IComparable<TriangleEdgeTriple>
    {
        public int vertexIndex0;
        public int vertexIndex1;
        public int triangeleIndex;

        public TriangleEdgeTriple(int vertexIndex0, int vertexIndex1, int triangeleIndex)
        {
            this.triangeleIndex = triangeleIndex;
            if (vertexIndex0 > vertexIndex1)
            {
                this.vertexIndex0 = vertexIndex1;
                this.vertexIndex1 = vertexIndex0;
            }
            else
            {
                this.vertexIndex0 = vertexIndex0;
                this.vertexIndex1 = vertexIndex1;
            }
        }

        public int CompareTo(TriangleEdgeTriple other)
        {
            if (vertexIndex0 == other.vertexIndex0)
            {
                if (vertexIndex1 == other.vertexIndex1)
                {
                    return 0;
                }
                else
                {
                    return vertexIndex1.CompareTo(other.vertexIndex1);
                }
            }
            else
            {
                return vertexIndex1.CompareTo(other.vertexIndex1);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TriangleEdgeTriple triple &&
                   vertexIndex0 == triple.vertexIndex0 &&
                   vertexIndex1 == triple.vertexIndex1 &&
                   triangeleIndex == triple.triangeleIndex;
        }

        public override int GetHashCode()
        {
            int hashCode = 128066273;
            hashCode = hashCode * -1521134295 + vertexIndex0.GetHashCode();
            hashCode = hashCode * -1521134295 + vertexIndex1.GetHashCode();
            hashCode = hashCode * -1521134295 + triangeleIndex.GetHashCode();
            return hashCode;
        }
    }
}