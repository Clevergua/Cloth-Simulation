namespace ClothSimulation
{
    internal struct Edge
    {
        public int vertexIndex0;
        public int vertexIndex1;

        public Edge(int vertexIndex0, int vertexIndex1)
        {
            this.vertexIndex0 = vertexIndex0;
            this.vertexIndex1 = vertexIndex1;
        }

        public override bool Equals(object obj)
        {
            return obj is Edge edge &&
                   vertexIndex0 == edge.vertexIndex0 &&
                   vertexIndex1 == edge.vertexIndex1;
        }

        public override int GetHashCode()
        {
            int hashCode = -929192685;
            hashCode = hashCode * -1521134295 + vertexIndex0.GetHashCode();
            hashCode = hashCode * -1521134295 + vertexIndex1.GetHashCode();
            return hashCode;
        }
    }
}