using System;
using UnityEngine;
namespace ClothSimulation
{
    [Serializable]
    public class FixedParticle
    {
        [SerializeField]
        public int index;
        [SerializeField]
        public Vector3 position;

        public FixedParticle(int index, Vector3 position)
        {
            this.index = index;
            this.position = position;
        }

        public override bool Equals(object obj)
        {
            return obj is FixedParticle particle &&
                   index == particle.index &&
                   position.Equals(particle.position);
        }

        public override int GetHashCode()
        {
            int hashCode = 488718701;
            hashCode = hashCode * -1521134295 + index.GetHashCode();
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            return hashCode;
        }
    }
}
