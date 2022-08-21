namespace ClothSimulation
{
    internal class BendContraintion
    {
        public int vertexIndex0;
        public int vertexIndex1;
        public int vertexIndex2;
        public int vertexIndex3;
        public float rest;

        public BendContraintion(int vertexIndex0, int vertexIndex1, int vertexIndex2, int vertexIndex3, float rest)
        {
            this.vertexIndex0 = vertexIndex0;
            this.vertexIndex1 = vertexIndex1;
            this.vertexIndex2 = vertexIndex2;
            this.vertexIndex3 = vertexIndex3;
            this.rest = rest;
        }

        public override bool Equals(object obj)
        {
            return obj is BendContraintion contraintion &&
                   vertexIndex0 == contraintion.vertexIndex0 &&
                   vertexIndex1 == contraintion.vertexIndex1 &&
                   vertexIndex2 == contraintion.vertexIndex2 &&
                   vertexIndex3 == contraintion.vertexIndex3 &&
                   rest == contraintion.rest;
        }

        public override int GetHashCode()
        {
            int hashCode = 1469733241;
            hashCode = hashCode * -1521134295 + vertexIndex0.GetHashCode();
            hashCode = hashCode * -1521134295 + vertexIndex1.GetHashCode();
            hashCode = hashCode * -1521134295 + vertexIndex2.GetHashCode();
            hashCode = hashCode * -1521134295 + vertexIndex3.GetHashCode();
            hashCode = hashCode * -1521134295 + rest.GetHashCode();
            return hashCode;
        }
    }
}