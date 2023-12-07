using eWolf.PipeBuilder.ExtraFittings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace eWolf.PipeBuilder.Editors
{
    [CustomEditor(typeof(Fittings))]
    [CanEditMultipleObjects]
    public class FittingsUI : Editor
    {
        private Fittings _pipeFitting;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.color = Color.green;
            if (GUILayout.Button("Build Mesh"))
            {
                _pipeFitting.MakeReady();
                _pipeFitting.CreateMesh();

                SetSceneDirty();
                return;
            }

            if (GUILayout.Button("Duplicate Fitting"))
            {
                Selection.objects = new GameObject[] { _pipeFitting.DuplicateFitting(_pipeFitting.transform.position) };

                SetSceneDirty();
                return;
            }
        }

        private void OnEnable()
        {
            _pipeFitting = (Fittings)target;
        }

        private void SetSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}