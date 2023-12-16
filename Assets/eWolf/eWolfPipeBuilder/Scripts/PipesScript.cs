using System.Collections.Generic;
using eWolf.PipeBuilder.Helpers;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using eWolf.PipeBuilder.Data;
using UnityEngine.UI;

namespace eWolf.PipeBuilder.VisionFlowScripts
{
    public class PipesScript : MonoBehaviour
    {

        private PipeNode _pipeNode;
        // Link to an pipe builder base so we will have all the setting we need.
        public PipeBase Pipe;
        public PipeNode currentPipeNode = null;
        public PipeSettings pipeSettings;
        public List<Vector3> nodesPositions;

        private void ClearAllPipes()
        {
            foreach (Transform child in Pipe.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        // private void CreateBasicPipe()
        // {
        //     GameObject go = Pipe.AddPipes();
        //     PipeNode pipeNode = go.GetComponent<PipeNode>();
        //     GameObject extendPipe = pipeNode.ExtendPipe();
        //     Pipe.BuildPipes();
        // }

        // private void CreateBasicPipe()
        // {
        //     currentPipeNode = Pipe.AddPipes().GetComponent<PipeNode>();
        //     PipeNode pn = obj.GetComponent<PipeNode>();
        //     Selection.objects = new GameObject[] { pn.ExtendPipe() };
        //     Debug.Log("Selected object: " + pn); // Add this line to log the selected object
        //     // return;

        // }

        private void AddNode()
        {
            if (_pipeNode.CanExtendPipes())
            {
                currentPipeNode = currentPipeNode.ExtendPipe().GetComponent<PipeNode>();
                // GameObject child = originalGameObject.transform.GetChild(0).gameObject;
                PipeBase pb = NodeHelper.GetPipeBase(_pipeNode.transform);
                pb.BuildPipes();
                Debug.Log("Selected object: " + pb); // Add this line to log the selected object
                // return;
            }
        }

        public void CreateOrExtendPipe()
        {
            GameObject parentObject = GameObject.Find("PipeBase_pf");
            Debug.Log(parentObject.transform.childCount);

            if (parentObject.transform.childCount == 0)
            {
                // Parent object doesn't exist, create a new pipe.
                Debug.Log("Creating a new pipe.");
                GameObject go = Pipe.AddPipes();
                PipeNode pipeNode = go.GetComponent<PipeNode>();
                GameObject extendPipe = pipeNode.ExtendPipe();
                Pipe.SetAllModifed();
                Pipe.BuildPipes();
            }
            else
            {
                // Parent object exists, extend the existing pipe.
                Debug.Log("Extending the existing pipe.");
                int childCount = parentObject.transform.childCount;
                Debug.Log("");
                if (childCount > 0)
                {
                    PipeNode[] pipeNodes = parentObject.GetComponentsInChildren<PipeNode>();
                    _pipeNode = pipeNodes[pipeNodes.Length - 1];
                    Debug.Log(_pipeNode);
                    _pipeNode.ExtendPipe();
                    Pipe.SetAllModifed();
                    Pipe.BuildPipes();
                    // lastChild = lastChild.ExtendPipe().GetComponent<PipeNode>();
                    // Your logic for extending the existing pipe goes here.
                }
                else
                {
                    Debug.Log("No children found.");
                }
            }
        }
        public Slider radiusSlider;
        public void ChangeRadiusOnMenu()
        {   
            Pipe.PipeSettings.Radius = radiusSlider.value/100/2; //a mm/ de diametro a radio
            Pipe.SetAllModifed();
            Pipe.BuildPipes();
        }
        
        public void CreateBasicPipeList()
        {
            // List<Vector3> positions = new List<Vector3>();
            // positions.Add(new Vector3(0, 0, 0));
            // positions.Add(new Vector3(4, 0, 0));
            // positions.Add(new Vector3(4, 0, 4));
            // positions.Add(new Vector3(0, 0, 4));
            // positions.Add(new Vector3(0, -4, 4));
            // positions.Add(new Vector3(0, -4, 0));

            GameObject go = Pipe.AddPipes();
            bool first = true;
            PipeNode currentPipeNode = null;
            foreach (Vector3 pos in nodesPositions)
            {
                if (first)
                {
                    currentPipeNode = Pipe.AddPipes().GetComponent<PipeNode>();
                    first = false;
                }
                else
                {
                    currentPipeNode = currentPipeNode.ExtendPipe().GetComponent<PipeNode>();
                }
                currentPipeNode.transform.position = pos;
            }
            Pipe.SetAllModifed();
            Pipe.BuildPipes();
        }

        private void Update()
        {
            Pipe.BuildPipes();
        }

        private void OnGUI()
        {
            int y = 10;
            if (GUI.Button(new Rect(10, y, 250, 45), "Create or Extend Pipe"))
            {
                CreateOrExtendPipe();
            }
            y += 50;
            if (GUI.Button(new Rect(10, y, 250, 45), "Clear Pipes"))
            {
                ClearAllPipes();
            }
            y += 50;
            if (GUI.Button(new Rect(10, y, 250, 45), "Create pipes from list"))
            {
                CreateBasicPipeList();
            }
        }
    }
}