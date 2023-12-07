using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingsBuilderBase
    {
        public Vector3 Direction;
        public FittingSettings FittingSetting;
        public GameObject GameObject;
        public Vector3 LeftDirection;
        public MeshBuilder MeshBuilder;
        public PipeBase PipeBase;
        public int Sides = 4;
        public Vector3 UpDirection;
        protected float _angleStep;
        private float _uVPoint = 0;

        public float UVPoint
        {
            get
            {
                return _uVPoint;
            }

            set
            {
                _uVPoint = value;
            }
        }

        public virtual void CreateMesh(Vector3 pipeSectionStart, float length, float radius)
        { }

        public void SetMaterial(GameObject obj, Material materialOther)
        {
            var render = obj.GetComponent<Renderer>();
            render.sharedMaterial = materialOther;
#if DEBUG
            if (PipeBase.PipeSettings.BakeLightingDetail.BakeLighting)
            {
                GameObjectUtility.SetStaticEditorFlags(obj, StaticEditorFlags.ContributeGI);
                render.shadowCastingMode = PipeBase.PipeSettings.BakeLightingDetail.ShadowCasting;
                render.receiveShadows = PipeBase.PipeSettings.BakeLightingDetail.ReceiveShadow;
                render.lightProbeUsage = LightProbeUsage.BlendProbes;
            }
#endif
        }

        protected void CreateFlange(Vector3 startOffSet, float radius, float flangeRadius, bool flipNormals, float angle)
        {
            CreateFlange(startOffSet, radius, flangeRadius, flipNormals, angle, LeftDirection, UpDirection);
        }

        protected void CreateFlange(Vector3 startOffSet, float radius, float flangeRadius, bool flipNormals, float angle, Vector3 leftDirection, Vector3 upDirection)
        {
            UVPoint += 0.01f;

            float uvSlice = 1 / (float)Sides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            float x2Outter = 0;
            float y2Outter = 0;

            float flangeUVSize = Mathf.Abs(flangeRadius - radius);

            UVSet uVSet = new UVSet(UVPoint, flangeUVSize);

            UVPoint += flangeUVSize;

            for (int i = 0; i < Sides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;
                float xOutter = Mathf.Sin(angle) * flangeRadius;
                float yOutter = -Mathf.Cos(angle) * flangeRadius;

                Vector3 a = (leftDirection * x + y * upDirection) + startOffSet;
                Vector3 b = (leftDirection * x2 + y2 * upDirection) + startOffSet;
                Vector3 c = (leftDirection * xOutter + yOutter * upDirection) + startOffSet;
                Vector3 d = (leftDirection * x2Outter + y2Outter * upDirection) + startOffSet;
                angle += _angleStep;

                x2 = x;
                y2 = y;
                x2Outter = xOutter;
                y2Outter = yOutter;

                if (i != 0)
                {
                    if (flipNormals)
                    {
                        MeshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        MeshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }

            UVPoint += 0.01f;
        }

        protected void CreateFrontPipe(Vector3 pipeSectionStart, float radius, ref float angle, UVSet uVSet, float length, Vector3 frontBackDirection, bool flip)
        {
            float uvSlice = 1 / (float)Sides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            for (int i = 0; i < Sides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;

                Vector3 a = (Direction * x + y * -UpDirection) + pipeSectionStart;
                Vector3 b = (Direction * x2 + y2 * -UpDirection) + pipeSectionStart;
                Vector3 c = (Direction * x + y * -UpDirection) + pipeSectionStart + (frontBackDirection * length);
                Vector3 d = (Direction * x2 + y2 * -UpDirection) + pipeSectionStart + (frontBackDirection * length);
                angle += _angleStep;

                x2 = x;
                y2 = y;

                if (i != 0)
                {
                    if (!flip)
                    {
                        MeshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        MeshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }
        }

        protected void CreateFrontPipeFlange(Vector3 startOffSet, float radius, float flangeRadius, bool flipNormals, float angle)
        {
            CreateFlange(startOffSet, radius, flangeRadius, flipNormals, angle, UpDirection, -Direction);
        }

        protected void CreatePipe(Vector3 pipeSectionStart, float radius, ref float angle, UVSet uVSet, float lengthHalf)
        {
            float uvSlice = 1 / (float)Sides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            for (int i = 0; i < Sides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;

                Vector3 a = (LeftDirection * x + y * UpDirection) + pipeSectionStart - (Direction * lengthHalf);
                Vector3 b = (LeftDirection * x2 + y2 * UpDirection) + pipeSectionStart - (Direction * lengthHalf);
                Vector3 c = (LeftDirection * x + y * UpDirection) + pipeSectionStart + (Direction * lengthHalf);
                Vector3 d = (LeftDirection * x2 + y2 * UpDirection) + pipeSectionStart + (Direction * lengthHalf);
                angle += _angleStep;

                x2 = x;
                y2 = y;

                if (i != 0)
                {
                    if (!PipeBase.PipeSettings.InsidePipe)
                    {
                        MeshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        MeshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }
        }

        protected void CreateVerticalPipe(Vector3 pipeSectionStart, float radius, ref float angle, UVSet uVSet, float length, Vector3 frontBackDirection, bool flip)
        {
            float uvSlice = 1 / (float)Sides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            for (int i = 0; i < Sides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;

                Vector3 a = (LeftDirection * x + y * -Direction) + pipeSectionStart;
                Vector3 b = (LeftDirection * x2 + y2 * -Direction) + pipeSectionStart;
                Vector3 c = (LeftDirection * x + y * -Direction) + pipeSectionStart + (frontBackDirection * length);
                Vector3 d = (LeftDirection * x2 + y2 * -Direction) + pipeSectionStart + (frontBackDirection * length);
                angle += _angleStep;

                x2 = x;
                y2 = y;

                if (i != 0)
                {
                    if (!flip)
                    {
                        MeshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        MeshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }
        }

        protected void CreateVerticalPipeFlange(Vector3 startOffSet, float radius, float flangeRadius, bool flipNormals, float angle)
        {
            CreateFlange(startOffSet, radius, flangeRadius, flipNormals, angle, Direction, -LeftDirection);
        }
    }
}