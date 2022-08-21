namespace ClothSimulation
{
    internal struct Spring
    {
        public int particeleIndex0;
        public int particeleIndex1;
        public float length;

        public Spring(int particeleIndex0, int particeleIndex1, float length)
        {
            this.particeleIndex0 = particeleIndex0;
            this.particeleIndex1 = particeleIndex1;
            this.length = length;
        }

        public override bool Equals(object obj)
        {
            return obj is Spring spring &&
                   particeleIndex0 == spring.particeleIndex0 &&
                   particeleIndex1 == spring.particeleIndex1 &&
                   length == spring.length;
        }

        public override int GetHashCode()
        {
            int hashCode = -1343789752;
            hashCode = hashCode * -1521134295 + particeleIndex0.GetHashCode();
            hashCode = hashCode * -1521134295 + particeleIndex1.GetHashCode();
            hashCode = hashCode * -1521134295 + length.GetHashCode();
            return hashCode;
        }
    }
}