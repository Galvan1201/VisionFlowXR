using eWolf.Common.Helper;
using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class Fittings : MonoBehaviour
    {
        public FittingSettings FittingSetting = new FittingSettings();
        public Material Material;
        public PipeBase PipeBase;
        private Vector3 _cachedPosition;

        private FittingDistance _fittingDistance;

        private MeshBuilder _meshBuilder = new MeshBuilder();

        public void ClearMesh()
        {
            ObjectHelper.RemoveComponent<MeshFilter>(gameObject);
            ObjectHelper.RemoveComponent<MeshRenderer>(gameObject);
            ObjectHelper.RemoveComponent<MeshCollider>(gameObject);

            ObjectHelper.RemoveAllObjectFrom(gameObject);
        }

        public void CreateMesh()
        {
            _meshBuilder = new MeshBuilder();

            var startPosition = _fittingDistance.NodeA.transform.position;
            var endPosition = _fittingDistance.NodeB.transform.position;

            FittingsBuilderBase fittingsBuilderBase = new FittingsBuilderBase();
            if (FittingSetting.FittingType == FittingTypes.Flange)
            {
                fittingsBuilderBase = new FittingsFlangeBuilder();
            }
            if (FittingSetting.FittingType == FittingTypes.Box)
            {
                fittingsBuilderBase = new FittingsBoxBuilder();
            }
            if (FittingSetting.FittingType == FittingTypes.ValveRound)
            {
                fittingsBuilderBase = new FittingsValveRoundBuilder();
            }
            if (FittingSetting.FittingType == FittingTypes.ValveLeaver)
            {
                fittingsBuilderBase = new FittingsValveLeaverBuilder();
            }

            fittingsBuilderBase.GameObject = gameObject;
            fittingsBuilderBase.FittingSetting = FittingSetting;
            fittingsBuilderBase.PipeBase = PipeBase;
            fittingsBuilderBase.Direction = endPosition - startPosition;
            fittingsBuilderBase.Direction.Normalize();
            fittingsBuilderBase.Sides = PipeBase.PipeSettings.Sides;
            fittingsBuilderBase.LeftDirection = Vector3.Cross(fittingsBuilderBase.Direction, Vector3.up);
            fittingsBuilderBase.UpDirection = Vector3.Cross(fittingsBuilderBase.Direction, fittingsBuilderBase.LeftDirection.normalized);
            fittingsBuilderBase.LeftDirection = Vector3.Cross(fittingsBuilderBase.Direction, fittingsBuilderBase.UpDirection);
            fittingsBuilderBase.UpDirection = Vector3.Cross(fittingsBuilderBase.Direction, fittingsBuilderBase.LeftDirection.normalized);
            fittingsBuilderBase.MeshBuilder = _meshBuilder;

            Vector3 pipeSectionStart = new Vector3();

            fittingsBuilderBase.CreateMesh(pipeSectionStart, FittingSetting.Length, PipeBase.PipeSettings.Radius + FittingSetting.ExtraRadius);

            bool applyOffSet = false;
            _meshBuilder.ApplyMeshDetails(gameObject, Material, applyOffSet, new LightingOptions(), null, PipeBase.PipeSettings);
        }

        public GameObject DuplicateFitting(Vector3 pos)
        {
            var obj = CreatorHelper.CreateFilters(pos, transform.parent);

            var fittings = obj.GetComponent<Fittings>();
            PipeBase pb = NodeHelper.GetPipeBase(transform);

            fittings.Material = pb.Material;
            fittings.PipeBase = pb;
            fittings.SetPosition(false);
            fittings.CreateMesh();

            fittings.FittingSetting.Length = FittingSetting.Length;
            fittings.FittingSetting.ExtraRadius = FittingSetting.ExtraRadius;
            fittings.FittingSetting.FittingType = FittingSetting.FittingType;
            fittings.FittingSetting.StemRadius = FittingSetting.StemRadius;
            fittings.FittingSetting.StemLength = FittingSetting.StemLength;
            fittings.FittingSetting.Direction = FittingSetting.Direction;
            fittings.FittingSetting.ObjectScaleModifier = FittingSetting.ObjectScaleModifier;

            return obj;
        }

        public void MakeReady()
        {
            var parent = transform.parent;
            PipeBase = parent.gameObject.GetComponent<PipeBase>();
            Material = PipeBase.Material;
            SetPosition(false);
        }

        public void OnDrawGizmosSelected()
        {
            var parent = transform.parent;

            NodeHelper.DrawAll(parent.transform.gameObject);

            if (_cachedPosition == transform.position)
                return;

            _cachedPosition = transform.position;
            SetPosition(true);

            CreateMesh();
        }

        public void SetPosition(bool draw)
        {
            var parent = transform.parent;
            PipeBase parentPipeNode = parent.gameObject.GetComponent<PipeBase>();
            Transform[] transforms = parentPipeNode.GetComponentsInChildren<Transform>();

            var currentPos = transform.position;

            PipeNode startingPipe = FindStartingNode(transforms);
            List<FittingDistance> fittingDistances = new List<FittingDistance>();

            fittingDistances.AddRange(CreateFittingList(currentPos, ref startingPipe, draw));

            fittingDistances = fittingDistances.OrderBy(x => x.Distance).ToList();
            transform.position = fittingDistances[0].Position;

            _fittingDistance = fittingDistances[0];
        }

        private static List<FittingDistance> CreateFittingList(Vector3 currentPos, ref PipeNode startingPipe, bool draw)
        {
            List<FittingDistance> fittingDistances = new List<FittingDistance>();
            PipeNode targetNode = startingPipe;

            int count = 16;
            while (startingPipe != null)
            {
                if (count-- == 0)
                    break;

                List<PipeNode> linkPipes = new List<PipeNode>();
                linkPipes.AddRange(startingPipe._pipes);

                List<PipeNode> pipesA = fittingDistances.Select(x => x.NodeA).ToList();
                List<PipeNode> pipesB = fittingDistances.Select(x => x.NodeB).ToList();

                linkPipes = linkPipes.Except(pipesA).ToList();
                linkPipes = linkPipes.Except(pipesB).ToList();

                if (!linkPipes.Any())
                {
                    // all links used
                    break;
                }

                if (linkPipes.Count == 1)
                {
                    targetNode = linkPipes[0];

                    fittingDistances.Add(new FittingDistance(startingPipe, targetNode, currentPos));
                    if (draw)
                        Gizmos.DrawLine(startingPipe.transform.position, targetNode.transform.position);

                    startingPipe = targetNode;
                }
                if (linkPipes.Count == 2)
                {
                    // two links from the link pipes - need to pick one and also pick the others as well.
                    Debug.LogError("2 links from the Node tree");
                }
            }
            return fittingDistances;
        }

        private static PipeNode FindStartingNode(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length - 1; i++)
            {
                PipeNode pipeNode = transforms[i].gameObject.GetComponent<PipeNode>();
                if (pipeNode == null)
                    continue;
                if (pipeNode._pipes.Count == 1)
                {
                    return pipeNode;
                }
            }

            return null;
        }
    }
}