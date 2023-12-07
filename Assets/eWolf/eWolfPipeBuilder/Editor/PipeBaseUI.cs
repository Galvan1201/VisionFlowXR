using UnityEditor;
using UnityEngine;

namespace eWolf.PipeBuilder.Editors
{
    [CustomEditor(typeof(PipeBase))]
    [CanEditMultipleObjects]
    public class PipeBaseUI : Editor
    {
        private readonly Texture _textureLogo = null;
        private PipeBase _pipeBase;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            /*if (_textureLogo == null)
                _textureLogo = (Texture)Resources.Load("PipeBuilder-Logo");

            GUILayout.Box(_textureLogo, GUILayout.Height(130), GUILayout.ExpandWidth(true));*/

            if (Selection.gameObjects.Length == 1)
            {
                if (GUILayout.Button("Generate Pipe Mesh"))
                {
                    _pipeBase.SetAllModifed();
                    _pipeBase.BuildPipes();
                    EditorUtility.SetDirty(target);
                    return;
                }
            }
            else
            {
                if (GUILayout.Button("Generate All Pipe Meshes"))
                {
                    foreach (var obj in Selection.gameObjects)
                    {
                        var pipeBase = obj.GetComponent<PipeBase>();
                        if (pipeBase != null)
                        {
                            pipeBase.SetAllModifed();
                            pipeBase.BuildPipes();
                        }
                    }
                    EditorUtility.SetDirty(target);
                    return;
                }
            }

            if (Selection.gameObjects.Length == 1)
            {
                if (GUILayout.Button("Add new pipe node"))
                {
                    GameObject obj = _pipeBase.AddPipes();
                    PipeNode pn = obj.GetComponent<PipeNode>();
                    Selection.objects = new GameObject[] { pn.ExtendPipe() };
                    EditorUtility.SetDirty(target);
                    return;
                }
            }

            if (_pipeBase.PipeSettings.BakeLightingDetail.BakeLighting)
            {
                if (GUILayout.Button("Generate bake Lighting"))
                {
                    Lightmapping.BakeAsync();
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Remove all Meshes"))
            {
                if (Selection.gameObjects.Length == 1)
                {
                    _pipeBase.ClearMesh();
                }
                else
                {
                    foreach (var obj in Selection.gameObjects)
                    {
                        var pipeBase = obj.GetComponent<PipeBase>();
                        if (pipeBase != null)
                        {
                            pipeBase.ClearMesh();
                        }
                    }
                }

                EditorUtility.SetDirty(target);
                return;
            }
        }

        private void OnEnable()
        {
            _pipeBase = (PipeBase)target;
        }
    }
}