using System;
using UnityEngine.Rendering;

namespace eWolf.PipeBuilder.Data
{
    [Serializable]
    public class BakeLightingDetails
    {
        public bool BakeLighting = false;

        public bool ReceiveShadow = false;
        public ShadowCastingMode ShadowCasting = ShadowCastingMode.Off;
    }
}