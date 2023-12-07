using eWolf.Common.Helper;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Helpers;
using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingsValveBaseBuilder : FittingsBuilderBase
    {
        public override void CreateMesh(Vector3 pipeSectionStart, float length, float radius)
        {
            float fullRadius = Mathf.PI * 2;
            _angleStep = fullRadius / (float)Sides;

            if (PipeBase.MaterialOther == null)
            {
                Debug.LogError("Please set the MaterialOther for the extra fitting models.");
            }

            if (FittingSetting.Direction == Data.Direction.Up)
                CreateMeshUp(pipeSectionStart, length, radius);

            if (FittingSetting.Direction == Data.Direction.Down)
                CreateMeshDown(pipeSectionStart, length, radius);

            if (FittingSetting.Direction == Data.Direction.Front)
                CreateMeshFront(pipeSectionStart, length, radius);

            if (FittingSetting.Direction == Data.Direction.Back)
                CreateMeshBack(pipeSectionStart, length, radius);
        }

        protected virtual GameObject CreateValueObject(Vector3 position, Transform parent, Vector3 rotation)
        {
            var go = CreatorHelper.CreateValveWheel(position, parent, rotation, FittingSetting.ObjectScaleModifier);

            if (PipeBase.PipeSettings.CollisionDetails.Style == CollisionStyles.None)
            {
                ObjectHelper.RemoveComponent<MeshCollider>(go);
            }
            return go;
        }

        private void CreateMainValveBody(Vector3 pipeSectionStart, float length, float radius)
        {
            float angle = Mathf.PI / 4;

            UVSet uVSet = new UVSet(UVPoint, length * PipeBase.PipeSettings.UVModifier);
            UVPoint += length * PipeBase.PipeSettings.UVModifier;

            float lengthHalf = length / 2;

            CreateFlange(pipeSectionStart + (Direction * lengthHalf), PipeBase.PipeSettings.Radius, radius, false, angle);
            CreateFlange(pipeSectionStart - (Direction * lengthHalf), PipeBase.PipeSettings.Radius, radius, true, angle);
            CreatePipe(pipeSectionStart, radius, ref angle, uVSet, lengthHalf);
        }

        private void CreateMeshBack(Vector3 pipeSectionStart, float length, float radius)
        {
            CreateMeshFront(pipeSectionStart, length, radius, -LeftDirection, true);
        }

        private void CreateMeshDown(Vector3 pipeSectionStart, float length, float radius)
        {
            CreateMeshUpDown(pipeSectionStart, length, radius, -UpDirection, true);
        }

        private void CreateMeshFront(Vector3 pipeSectionStart, float length, float radius)
        {
            CreateMeshFront(pipeSectionStart, length, radius, LeftDirection, false);
        }

        private void CreateMeshFront(Vector3 pipeSectionStart, float length, float radius, Vector3 frontBackDirection, bool flip)
        {
            CreateMainValveBody(pipeSectionStart, length, radius);

            var uVSet = new UVSet(UVPoint, length * PipeBase.PipeSettings.UVModifier);
            UVPoint += length * PipeBase.PipeSettings.UVModifier;

            float angle = Mathf.PI / 4;
            CreateFrontPipeFlange(pipeSectionStart + (frontBackDirection * (radius * 2f)), FittingSetting.StemRadius, radius, !flip, angle);
            CreateFrontPipe(pipeSectionStart, radius, ref angle, uVSet, radius * 2f, frontBackDirection, flip);

            var pos = pipeSectionStart + (frontBackDirection * (radius * 2f));
            CreateFrontPipe(pos, FittingSetting.StemRadius, ref angle, uVSet, FittingSetting.StemLength, frontBackDirection, flip);

            var flangepos = pos + (frontBackDirection * (FittingSetting.StemLength));
            pos = pos - (frontBackDirection * (FittingSetting.StemLength));
            CreateFrontPipeFlange(flangepos, FittingSetting.StemRadius, 0.01f, flip, angle);

            ObjectHelper.RemoveAllObjectFrom(GameObject);

            Vector3 wheelPos = GameObject.transform.position - pos;
            wheelPos += frontBackDirection * (radius * 4);
            GameObject obj = CreateValueObject(wheelPos, GameObject.transform, frontBackDirection);
            obj.transform.Rotate(new Vector3(90, 0, 0));

            SetMaterial(obj, PipeBase.MaterialOther);
        }

        private void CreateMeshUp(Vector3 pipeSectionStart, float length, float radius)
        {
            CreateMeshUpDown(pipeSectionStart, length, radius, UpDirection, false);
        }

        private void CreateMeshUpDown(Vector3 pipeSectionStart, float length, float radius, Vector3 frontBackDirection, bool flip)
        {
            CreateMainValveBody(pipeSectionStart, length, radius);

            var uVSet = new UVSet(UVPoint, length * PipeBase.PipeSettings.UVModifier);
            UVPoint += length * PipeBase.PipeSettings.UVModifier;

            float angle = Mathf.PI / 4;
            CreateVerticalPipeFlange(pipeSectionStart + (frontBackDirection * (radius * 2f)), FittingSetting.StemRadius, radius, !flip, angle);
            CreateVerticalPipe(pipeSectionStart, radius, ref angle, uVSet, radius * 2f, frontBackDirection, flip);

            var pos = pipeSectionStart + (frontBackDirection * (radius * 2f));
            CreateVerticalPipe(pos, FittingSetting.StemRadius, ref angle, uVSet, FittingSetting.StemLength, frontBackDirection, flip);

            var flangepos = pos + (frontBackDirection * (FittingSetting.StemLength));
            pos = pos - (frontBackDirection * (FittingSetting.StemLength));
            CreateVerticalPipeFlange(flangepos, FittingSetting.StemRadius, 0.01f, flip, angle);

            ObjectHelper.RemoveAllObjectFrom(GameObject);

            Vector3 wheelPos = GameObject.transform.position - pos;

            wheelPos += frontBackDirection * (radius * 4);
            var obj = CreateValueObject(wheelPos, GameObject.transform, Direction);
            if (flip)
                obj.transform.Rotate(new Vector3(180, 90, 0));
            else
                obj.transform.Rotate(new Vector3(0, 90, 0));

            SetMaterial(obj, PipeBase.MaterialOther);
        }
    }
}