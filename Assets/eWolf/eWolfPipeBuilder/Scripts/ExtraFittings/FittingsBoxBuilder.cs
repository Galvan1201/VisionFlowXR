using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingsBoxBuilder : FittingsFlangeBuilder
    {
        public override void CreateMesh(Vector3 pipeSectionStart, float length, float radius)
        {
            Sides = 4;
            base.CreateMesh(pipeSectionStart, length, radius);
        }
    }
}