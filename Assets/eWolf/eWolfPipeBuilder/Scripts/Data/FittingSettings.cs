using System;

namespace eWolf.PipeBuilder.Data
{
    [Serializable]
    public class FittingSettings
    {
        public Direction Direction = Direction.Up;
        public float ExtraRadius = 0.2f;
        public FittingTypes FittingType = FittingTypes.Flange;
        public float Length = 4;

        public float ObjectScaleModifier = 1f;
        public float StemLength = 0.25f;
        public float StemRadius = 0.2f;
    }
}