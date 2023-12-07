using System;

namespace eWolf.PipeBuilder.Data
{
    [Serializable]
    public class PipeSettings
    {
        public bool AutoBuild = true;
        public BakeLightingDetails BakeLightingDetail = new BakeLightingDetails();
        public CollisionDetails CollisionDetails = new CollisionDetails();
        public JointDetails CornersDetail = new JointDetails();
        public FlangeDetails FlangeDetail = new FlangeDetails();
        public bool InsidePipe = false;
        public float Radius = 2;
        public int Sides = 9;
        public float UVModifier = 1;

        public float FlangeRadius
        {
            get
            {
                return Radius + FlangeDetail.Size;
            }
        }

        public float JoinRadius
        {
            get
            {
                if (CornersDetail.Larger)
                {
                    return FlangeRadius;
                }
                else
                {
                    return Radius;
                }
            }
        }
    }
}