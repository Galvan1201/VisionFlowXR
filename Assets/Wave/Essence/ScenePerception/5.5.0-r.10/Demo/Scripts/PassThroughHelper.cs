using System;
using System.Collections.Generic;
using UnityEngine;
using Wave.Native;

namespace Wave.Essence.ScenePerception.Sample
{
    [Serializable]
    public class PassThroughHelper : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> RoomPanelMeshRenderers = new List<MeshRenderer>(5);
        [SerializeField] private Material DemoRoomMatOpaque, DemoRoomMatTransparent;
        [SerializeField] private Camera hmd;

        
        private ScenePerceptionMeshFacade _scenePerceptionMeshFacade;

        public void ShowPassthroughUnderlay(bool show)
        {
        
                //Set Demo Room Material to transparent and clear skybox to transparent black
                foreach (MeshRenderer meshRender in RoomPanelMeshRenderers)
                {
                    meshRender.material = DemoRoomMatTransparent;
                }

                hmd.clearFlags = CameraClearFlags.SolidColor;
                hmd.backgroundColor = new Color(0, 0, 0, 0);
                Interop.WVR_ShowPassthroughUnderlay(true);
                Interop.WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality.QualityMode);
                
                
                _scenePerceptionMeshFacade.SetVisibility(false);
        }

        public void HidePassthroughUnderlay()
        {
                foreach (MeshRenderer meshRender in RoomPanelMeshRenderers)
                {
                    meshRender.material = DemoRoomMatOpaque;
                }

                hmd.clearFlags = CameraClearFlags.SolidColor;
                hmd.backgroundColor = new Color(0, 0, 0, 0);
                Interop.WVR_ShowPassthroughUnderlay(false);
                _scenePerceptionMeshFacade.SetVisibility(true);

        }
    }
}
