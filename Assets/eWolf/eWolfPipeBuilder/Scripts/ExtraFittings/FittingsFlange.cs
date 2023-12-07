using eWolf.PipeBuilder.Data;
using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingsFlangeBuilder : FittingsBuilderBase
    {
        public override void CreateMesh(Vector3 pipeSectionStart, float length, float radius)
        {
            float fullRadius = Mathf.PI * 2;
            _angleStep = fullRadius / (float)Sides;

            float angle = Mathf.PI / 4;

            UVSet uVSet = new UVSet(UVPoint, length * PipeBase.PipeSettings.UVModifier);

            UVPoint += length * PipeBase.PipeSettings.UVModifier;

            float lengthHalf = length / 2;

            CreateFlange(pipeSectionStart + (Direction * lengthHalf), PipeBase.PipeSettings.Radius, radius, false, angle);
            CreateFlange(pipeSectionStart - (Direction * lengthHalf), PipeBase.PipeSettings.Radius, radius, true, angle);
            CreatePipe(pipeSectionStart, radius, ref angle, uVSet, lengthHalf);
        }
    }
}