using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.PipeBuilders;
using UnityEngine;

namespace eWolf.PipeBuilder.Sections
{
    public abstract class SectionStraightBase
    {
        public string ConbindedName { get; set; }
        public GameObject GameObject { get; set; }

        public PipeBase PipeBase { get; set; }

        public PipeBuilderBase PipeBuilderBase { get; set; }
        public PipeNode PipeNode { get; set; }

        public abstract float Build(float uvPosition);

        protected void ApplyCollisions(PipeBuilderBase pbs, GameObject gameObject)
        {
            if (PipeBase.PipeSettings.CollisionDetails.Style == CollisionStyles.HighRes)
            {
                pbs.ApplyCollision(gameObject);
            }
            else
            {
                pbs.RemoveCollision(gameObject);
            }
        }
    }
}