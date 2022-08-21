using UnityEngine;

namespace ClothSimulation
{
    internal class Particle
    {
        public int vertexIndex;
        public Vector3 position;
        public Vector3 velocity;
        internal bool movable;

        public Particle(int vertexIndex, Vector3 position, Vector3 velocity, bool movable)
        {
            this.vertexIndex = vertexIndex;
            this.position = position;
            this.velocity = velocity;
            this.movable = movable;
        }

        public override bool Equals(object obj)
        {
            return obj is Particle particle &&
                   vertexIndex == particle.vertexIndex &&
                   position.Equals(particle.position) &&
                   velocity.Equals(particle.velocity) &&
                   movable == particle.movable;
        }

        public override int GetHashCode()
        {
            int hashCode = 1973915322;
            hashCode = hashCode * -1521134295 + vertexIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            hashCode = hashCode * -1521134295 + velocity.GetHashCode();
            hashCode = hashCode * -1521134295 + movable.GetHashCode();
            return hashCode;
        }
    }
}