using eWolf.Common.Helper;
using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using System.Collections.Generic;
using UnityEngine;

namespace eWolf.PipeBuilder.PipeBuilders
{
    public class PipeBuilderBase
    {
        protected readonly MeshBuilder _meshBuilder;

        protected readonly PipeBase _pipeBase;

        private float uVPoint = 0;

        public PipeBuilderBase()
        {
        }

        public PipeBuilderBase(PipeBuilderBase pipeBase)
        {
            _meshBuilder = pipeBase._meshBuilder;
            _pipeBase = pipeBase._pipeBase;
        }

        public PipeBuilderBase(MeshBuilder meshBuilder, PipeBase pipeBase)
        {
            _meshBuilder = meshBuilder;
            _pipeBase = pipeBase;
        }

        public float UVPoint
        {
            get
            {
                return uVPoint;
            }

            set
            {
                uVPoint = value;
            }
        }

        public void ApplyCollision(GameObject gameObject)
        {
            _meshBuilder.ApplyCollision(gameObject, _pipeBase.PipeSettings);
        }

        public void ApplyMeshDetails(GameObject gameObject, Material material, bool applyOffSet, LightingOptions lightingOptions, List<Material> allMaterials)
        {
            _meshBuilder.ApplyMeshDetails(gameObject, material, applyOffSet, lightingOptions, allMaterials, _pipeBase.PipeSettings);
        }

        public void RemoveCollision(GameObject baseObject)
        {
            ObjectHelper.RemoveComponent<MeshCollider>(baseObject);
        }

        protected (float xFirst, float yFirst) GetAngleSet(float angle, float radius)
        {
            float x = Mathf.Sin(angle) * radius;
            float y = -Mathf.Cos(angle) * radius;
            return (x, y);
        }
    }
}