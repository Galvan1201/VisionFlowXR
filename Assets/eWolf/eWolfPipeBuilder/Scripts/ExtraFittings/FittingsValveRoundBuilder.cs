using eWolf.Common.Helper;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Helpers;
using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingsValveRoundBuilder : FittingsValveBaseBuilder
    {
        public override void CreateMesh(Vector3 pipeSectionStart, float length, float radius)
        {
            base.CreateMesh(pipeSectionStart, length, radius);
        }

        protected override GameObject CreateValueObject(Vector3 position, Transform parent, Vector3 rotation)
        {
            var go = CreatorHelper.CreateValveWheel(position, parent, rotation, FittingSetting.ObjectScaleModifier);

            if (PipeBase.PipeSettings.CollisionDetails.Style == CollisionStyles.None)
            {
                ObjectHelper.RemoveComponent<MeshCollider>(go);
            }
            return go;
        }
    }
}