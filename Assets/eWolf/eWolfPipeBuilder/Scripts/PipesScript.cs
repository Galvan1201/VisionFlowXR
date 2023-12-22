using System.Collections.Generic;
using eWolf.PipeBuilder.Helpers;
using UnityEngine;
using eWolf.PipeBuilder.Data;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;

namespace eWolf.PipeBuilder.VisionFlowScripts
{
    public class PipesScript : MonoBehaviour
    {
        public bool isEditing = true;
        public GameObject nodeEditMode;
        private PipeNode _pipeNode;
        // Link to an pipe builder base so we will have all the setting we need.
        public PipeBase Pipe;
        public PipeNode currentPipeNode = null;
        public PipeSettings pipeSettings;
        public List<Vector3> nodesPositions;
        private List<Vector3> newNodesPositions;
        public List<PipeNode> selectedNodes;
        public float radius;
        public GlobalVariables globalVariables;

        // private void ClearAllPipes()
        // {
        //     foreach (Transform child in Pipe.transform)
        //     {
        //         GameObject.Destroy(child.gameObject);
        //     }
        // }


        public void ToggleEditMode(bool editMode)
        {
            Debug.Log("EditMode toggled to: " + editMode);

            foreach (Transform child in transform)
            {
                // Check if the child has already been instanced
                if (child.childCount != 0)
                {
                    // Child has already been instanced, toggle its visibility
                    GameObject childPrefab = child.GetChild(0).gameObject;
                    childPrefab.SetActive(editMode);
                    Debug.Log("Prefab visibility toggled to: " + editMode);
                }
            }
            isEditing = !editMode;
        }

        // Regenerates the spheres placed on the nodes, updates the nodes list and deselects all nodes.
        public void UpdateEditNodes()
        {
            // Deselects all nodes
            selectedNodes.Clear();
            nodesPositions.Clear();
            foreach (Transform node in transform)
            {
                // Check if the child has already been instanced
                if (node.childCount == 0)
                {
                    // node hasn't been instanced, instantiate the prefab
                    GameObject instantiatedPrefab = Instantiate(nodeEditMode, node.position, node.rotation, node);
                    nodeEditMode.transform.localScale = new Vector3(radius * 2.5f, radius * 2.5f, radius * 2.5f);
                    XRGrabInteractable grabInteractable = node.AddComponent<XRGrabInteractable>();
                    grabInteractable.attachTransform = instantiatedPrefab.transform;
                    grabInteractable.useDynamicAttach = true;
                    grabInteractable.trackRotation = false;
                    grabInteractable.trackScale = false;
                    grabInteractable.throwOnDetach = false;
                    grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
                    Rigidbody nodeRigidbody = node.GetComponent<Rigidbody>();
                    nodeRigidbody.useGravity = false;
                    nodeRigidbody.isKinematic = true;
                }
                else
                {
                    //Changes node sphere material to deselected
                    NodeEditModeLogic nodeEditMode = node.GetComponentInChildren<NodeEditModeLogic>();
                    nodeEditMode.gameObject.GetComponent<Renderer>().material = nodeEditMode.deselectedMat;
                }
                nodesPositions.Add(node.position);
            }
        }
        // Call this function when rebuilding the pipe is necessary
        // public void UpdateNodesList()
        // {
        //     foreach (Transform node in transform)
        //     {
        //         // insert the node position into the list
        //         newNodesPositions.Add(node.position);
        //     }
        //     nodesPositions = newNodesPositions;
        //     newNodesPositions.Clear();
        // }

        public void AddNode()
        {
            if (selectedNodes[0].CanExtendPipes())
            {
                selectedNodes[0].ExtendPipe().GetComponent<PipeNode>();
                Pipe.SetAllModifed();
                Pipe.BuildPipes();
                UpdateEditNodes();
            }
        }

        //Inserts a node between two nodes, no matter if they are connected or not
        public void InsertNodeInBetween()
        {
            if (selectedNodes.Count == 2)
            {
                Pipe.InsertNode(selectedNodes);
                Pipe.SetAllModifed();
                Pipe.BuildPipes();
                UpdateEditNodes();
            }
        }

        public void DeleteNode()
        {
            Pipe.RemoveNode(selectedNodes[0]);
            DestroyImmediate(selectedNodes[0].gameObject);
            Pipe.SetAllModifed();
            Pipe.BuildPipes();
            UpdateEditNodes();
        }

        // public void CreateOrExtendPipe()
        // {
        //     GameObject parentObject = GameObject.Find("PipeBase_pf");
        //     Debug.Log(parentObject.transform.childCount);

        //     if (parentObject.transform.childCount == 0)
        //     {
        //         // Parent object doesn't exist, create a new pipe.
        //         Debug.Log("Creating a new pipe.");
        //         GameObject go = Pipe.AddPipes();
        //         PipeNode pipeNode = go.GetComponent<PipeNode>();
        //         GameObject extendPipe = pipeNode.ExtendPipe();
        //         Pipe.SetAllModifed();
        //         Pipe.BuildPipes();
        //     }
        //     else
        //     {
        //         // Parent object exists, extend the existing pipe.
        //         Debug.Log("Extending the existing pipe.");
        //         int childCount = parentObject.transform.childCount;
        //         Debug.Log("");
        //         if (childCount > 0)
        //         {
        //             PipeNode[] pipeNodes = parentObject.GetComponentsInChildren<PipeNode>();
        //             _pipeNode = pipeNodes[pipeNodes.Length - 1];
        //             Debug.Log(_pipeNode);
        //             _pipeNode.ExtendPipe();
        //             Pipe.SetAllModifed();
        //             Pipe.BuildPipes();
        //             // lastChild = lastChild.ExtendPipe().GetComponent<PipeNode>();
        //             // Your logic for extending the existing pipe goes here.
        //         }
        //         else
        //         {
        //             Debug.Log("No children found.");
        //         }
        //     }
        // }
        // public Slider radiusSlider;
        // public void ChangeRadiusOnMenu()
        // {
        //     Pipe.PipeSettings.Radius = radiusSlider.value / 100 / 2; //a mm/ de diametro a radio
        //     Pipe.SetAllModifed();
        //     Pipe.BuildPipes();
        // }


        //Called when need of create an intial pipe from a list
        public void CreateInitialPipeFromList()
        {
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
            Destroy(transform.GetChild(0).gameObject);
            Pipe.SetAllModifed();
            Pipe.BuildPipes();
            UpdateEditNodes();
            globalVariables = GameObject.Find("GlobalVariables").GetComponent<GlobalVariables>();
            globalVariables.pipeBeingEdited = gameObject;
        }

        private void Update()
        {
            // Pipe.BuildPipes();
        }

        // private void OnGUI()
        // {
        //     int y = 10;
        //     if (GUI.Button(new Rect(10, y, 250, 45), "Create or Extend Pipe"))
        //     {
        //         CreateOrExtendPipe();
        //     }
        //     y += 50;
        //     if (GUI.Button(new Rect(10, y, 250, 45), "Clear Pipes"))
        //     {
        //         ClearAllPipes();
        //     }
        //     y += 50;
        //     if (GUI.Button(new Rect(10, y, 250, 45), "Create pipes from list"))
        //     {
        //         CreateBasicPipeList();
        //     }
        // }
    }
}