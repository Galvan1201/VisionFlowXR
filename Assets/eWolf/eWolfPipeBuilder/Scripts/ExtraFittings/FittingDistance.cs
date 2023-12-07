using eWolf.PipeBuilder.Helpers;
using UnityEngine;

namespace eWolf.PipeBuilder.ExtraFittings
{
    public class FittingDistance
    {
        public float Distance;
        public PipeNode NodeA;
        public PipeNode NodeB;
        public Vector3 Position;

        public FittingDistance(PipeNode startingPipe, PipeNode targetNode, Vector3 currentPos)
        {
            var pos = MathsHelper.GetClosestPointOnLineSegment(startingPipe.transform.position, targetNode.transform.position, currentPos);

            NodeA = startingPipe;
            NodeB = targetNode;
            Position = pos;
            Distance = (pos - currentPos).magnitude;
        }
    }
}