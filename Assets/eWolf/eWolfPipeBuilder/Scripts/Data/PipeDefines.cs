using UnityEngine;

namespace eWolf.PipeBuilder.Data
{
    public class PipeDefines
    {
        public Vector3 CornerStartOffSet;
        public Vector3 Direction;
        public Vector3 Left;
        public float Length;
        public Vector3 Up;
        private readonly PipeNode _pipeBase;
        private readonly PipeNode _pipeNode;

        public PipeDefines(PipeNode pipeNode, PipeNode pipeBase, float startOffSet)
        {
            _pipeNode = pipeNode;
            _pipeBase = pipeBase;

            ProcessPipeDirections();

            CornerStartOffSet = pipeNode.GetOffSetFrom(pipeBase.transform.position, startOffSet);
        }

        public Vector3 Position
        {
            get
            {
                return _pipeNode.transform.position;
            }
        }

        private void ProcessPipeDirections()
        {
            Direction = _pipeNode.GetDirection(_pipeBase.transform.position);

            Length = _pipeNode.GetLength(_pipeBase.transform.position);

            Left = Vector3.Cross(Direction, Vector3.up);
            Up = Vector3.Cross(Direction, Left.normalized);
            Left = Vector3.Cross(Direction, Up);
            Up = Vector3.Cross(Direction, Left.normalized);
        }
    }
}