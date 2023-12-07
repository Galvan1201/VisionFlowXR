using eWolf.PipeBuilder.Helpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace eWolf.PipeBuilder.Editors
{
    [CustomEditor(typeof(PipeNode))]
    [CanEditMultipleObjects]
    public class PipeNodeUI : Editor
    {
        private PipeNode _pipeNode;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Pipe Mesh"))
            {
                PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                pb.BuildPipes();
                SetSceneDirty();
                return;
            }

            if (Selection.gameObjects.Length == 1)
            {
                if (_pipeNode.CanExtendPipes())
                {
                    if (GUILayout.Button("Extend Pipe"))
                    {
                        Selection.objects = new GameObject[] { _pipeNode.ExtendPipe() };
                        SetSceneDirty();
                        return;
                    }
                }
            }

            if (Selection.gameObjects.Length == 2)
            {
                if (GUILayout.Button("Insert Node Between"))
                {
                    PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                    pb.ClearMesh();
                    List<PipeNode> nodes = GetSelectNodes();

                    if (nodes.Count == 2)
                    {
                        pb.InsertNode(nodes);
                        pb.BuildPipes();
                        SetSceneDirty();
                        EditorGUIUtility.ExitGUI();
                    }
                }
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Add Fittings"))
            {
                List<PipeNode> nodes = GetSelectNodes();

                GameObject fitting;
                if (nodes.Count == 2)
                {
                    fitting = _pipeNode.AddFilter(nodes);
                }
                else
                {
                    fitting = _pipeNode.AddFilter();
                }
                Selection.objects = new GameObject[] { fitting };
                SetSceneDirty();
                return;
            }

            if (Selection.gameObjects.Length == 1)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Delete Node"))
                {
                    PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                    pb.RemoveNode(_pipeNode);

                    DestroyImmediate(_pipeNode.gameObject);

                    pb.SetAllModifed();
                    pb.BuildPipes();
                    SetSceneDirty();
                    EditorGUIUtility.ExitGUI();
                }
            }
        }

        private static List<PipeNode> GetSelectNodes()
        {
            List<PipeNode> nodes = new List<PipeNode>();
            foreach (GameObject o in Selection.gameObjects)
            {
                var rnn = o.GetComponent<PipeNode>();
                if (rnn != null)
                    nodes.Add(rnn);
            }

            return nodes;
        }

        private void OnEnable()
        {
            _pipeNode = (PipeNode)target;
        }

        private void SetSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}