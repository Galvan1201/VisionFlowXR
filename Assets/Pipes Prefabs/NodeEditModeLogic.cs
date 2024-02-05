using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.Animations;
using eWolf.PipeBuilder.Helpers;
using System;
using UnityEngine.Rendering;
using eWolf.PipeBuilder.Data;


public class NodeEditModeLogic : MonoBehaviour
{
    private PipeBase parentPipe;
    private WristUI wristUI;
    private GameObject parentNodeObject;
    private PipeNode parentNodeScript;
    public Material originalMaterial;
    //Add logic variables
    private GameObject addObject;
    private Button addButton;
    private PipeNode newPipeNode;
    //Remove logic variables
    private GameObject removeObject;
    private Button deleteButton;

    // Split or create section logic    
    private Button splitButton;
    private PipesScript pipesScript;
    private XRGrabInteractable grabInteractable;
    [SerializeField] public Material deselectedMat;
    [SerializeField] public Material selectedMat;
    private new Renderer renderer;


    //SelectLogicVariables
    private Button selectNode;

    // Start is called before the first frame update
    void Start()
    {
        parentNodeObject = transform.parent.gameObject;
        parentNodeScript = parentNodeObject.GetComponent<PipeNode>();
        parentPipe = transform.parent.GetComponentInParent<PipeBase>();
        addObject = transform.Find("Editables").Find("Add").gameObject;
        removeObject = transform.Find("Editables").Find("Remove").gameObject;
        pipesScript = transform.parent.GetComponentInParent<PipesScript>();
        grabInteractable = transform.parent.GetComponent<XRGrabInteractable>();
        renderer = gameObject.GetComponent<Renderer>();

        wristUI = FindAnyObjectByType<WristUI>();
        addButton = wristUI.gameObject.transform.Find("Add Node").GetComponentInChildren<Button>();
        splitButton = wristUI.gameObject.transform.Find("Split Node").GetComponentInChildren<Button>();
        deleteButton = wristUI.gameObject.transform.Find("Delete Node").GetComponentInChildren<Button>();
        // Debug.Log(addButton);
        // addButton.onClick.AddListener(AddNode);

        selectNode = gameObject.GetComponentInChildren<Button>();
        selectNode.onClick.AddListener(SetSelectedNodes);
        originalMaterial = parentPipe.Material;
    }

    // public void AddNode()
    // {
    //     if (parentNodeScript.CanExtendPipes())
    //     {
    //         newPipeNode = parentNodeScript.ExtendPipe().GetComponent<PipeNode>();
    //         parentPipe.SetAllModifed();
    //         parentPipe.BuildPipes();
    //         pipesScript.UpdateEditNodes();
    //         Debug.Log("Node Added: " + newPipeNode);
    //     }
    // }

    public void SetSelectedNodes()
    {
        if (pipesScript.selectedNodes.Contains(parentNodeScript))
        {
            pipesScript.selectedNodes.Remove(parentNodeScript);
            //ADD WARNING NEW NODE ADDED
            Debug.Log("Node Removed: " + parentNodeScript);
            renderer.material = deselectedMat;
        }
        else
        {
            if (pipesScript.selectedNodes.Count < 2)
            {
                pipesScript.selectedNodes.Add(parentNodeScript);
                //ADD WARNING NEW NODE REMOVED
                renderer.material = selectedMat;
            }
            else
            {
                //ADD WARNING
                pipesScript.globalVariables.alert.Alert("You can only select 2 nodes", 1f, MessageType.Error);
            }
        }
        if (pipesScript.selectedNodes.Count == 0)
        {
            addButton.interactable = false;
            splitButton.interactable = false;
            deleteButton.interactable = false;
        }
        if (pipesScript.selectedNodes.Count == 1)
        {
            addButton.interactable = true;
            splitButton.interactable = false;
            deleteButton.interactable = true;
        }
        if (pipesScript.selectedNodes.Count == 2)
        {
            addButton.interactable = false;
            splitButton.interactable = true;
            deleteButton.interactable = false;
        }
    }

    //Change if necessary
    // private float collisionRadius = 0.1f;
    // void CheckForCollision()
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius);
    //     bool isColliding = false;

    //     foreach (Collider hitCollider in hitColliders)
    //     {
    //         if (hitCollider.gameObject.layer == 2)
    //         {
    //             isColliding = true;
    //             // break; // Exit the loop early if a collision is found
    //         }
    //     }
    //     if (isColliding)
    //     {
    //         if (parentPipe.Material = originalMaterial)
    //         {
    //             parentPipe.Material = parentPipe.collisionMaterial;
    //         }
    //     }
    //     else
    //     {
    //         if (parentPipe.Material = parentPipe.collisionMaterial)
    //         {
    //             parentPipe.Material = originalMaterial;
    //         }
    //     }
    // }


    // Update is called once per frame
    void Update()
    {
        if (grabInteractable.isSelected)
        {   
            // CheckForCollision();
            parentPipe.SetAllModifed();
            parentPipe.BuildPipes();
        }
    }
}
