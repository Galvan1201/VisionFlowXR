using System.Collections.Generic;
using eWolf.PipeBuilder.Helpers;
using UnityEngine;
using eWolf.PipeBuilder.Data;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.Utilities;

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
        public float totalLength;
        public float totalCost;
        float materialCost;

        public void ToggleEditMode(bool editMode)
        {
            foreach (Transform child in transform)
            {
                // Check if the child has already been instanced
                if (child.childCount != 0)
                {
                    // Child has already been instanced, toggle its visibility
                    GameObject childPrefab = child.GetChild(0).gameObject;
                    childPrefab.SetActive(editMode);
                }
                child.GetComponent<XRGrabInteractable>().enabled = editMode;
            }
            isEditing = editMode;
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

        public void CalculateTotalLength()
        {
            totalLength = 0f;
            List<float> lengths = new List<float>();
            foreach (Transform node in transform)
            {
                foreach (PipeNode mate in node.GetComponent<PipeNode>()._pipes)
                {
                    if (!lengths.Contains((mate.transform.position - node.position).magnitude))
                    {
                        lengths.Add((mate.transform.position - node.position).magnitude);
                        totalLength += (mate.transform.position - node.position).magnitude;
                    }
                }
            }
            //Debug.Log($"Total Length {totalLength}");
        }


        public void CalculateCost()
        {
            switch (Pipe.Material.name.Replace(" (Instance)", ""))
            {
                case "Aluminum":
                    materialCost = 0.3f;
                    break;
                case "Brass":
                    materialCost = 1.25f;
                    break;
                case "Cast Iron":
                    materialCost = 0.46f;
                    break;
                case "Concrete":
                    materialCost = 0.256f;
                    break;
                case "Copper":
                    materialCost = 0.69f;
                    break;
                case "CPVC":
                    materialCost = 0.43f;
                    break;
                case "Glass fiber":
                    materialCost = 1.34f;
                    break;
                case "PVC":
                    materialCost = 0.0715f;
                    break;
                case "Steel (regular)":
                    materialCost = 0.18f;
                    break;
                case "Steel (stainless)":
                    materialCost = 0.80f;
                    break;
                default:
                    Debug.Log("Default Steel");
                    materialCost = 0.18f;
                    break;
            }
            totalCost = totalLength * radius*1000 * materialCost * 1.3f;
        }


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
        }
    }
}