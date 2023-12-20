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


public class NodeEditModeLogic : MonoBehaviour
{
    private PipeBase parentPipe;
    private WristUI wristUI;
    private GameObject parentNodeObject;
    private PipeNode parentNodeScript;
    //Add logic variables
    private GameObject addObject;
    private Button addButton;
    private PipeNode newPipeNode;
    //Remove logic variables
    private GameObject removeObject;
    private PipesScript pipesScript;
    private XRGrabInteractable grabInteractable;
    [SerializeField] private Material deselectedMat;
    [SerializeField] private Material selectedMat;
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
        Debug.Log(wristUI);
        addButton = wristUI.gameObject.transform.Find("Add Node").GetComponent<Button>();
        // Debug.Log(addButton);
        // addButton.onClick.AddListener(AddNode);

        selectNode = gameObject.GetComponentInChildren<Button>();
        selectNode.onClick.AddListener(SetSelectedNodes);
    }   

    public void AddNode()
    {
        Debug.Log("Entra");
        if (parentNodeScript.CanExtendPipes())
        {
            newPipeNode = parentNodeScript.ExtendPipe().GetComponent<PipeNode>();
            parentPipe.SetAllModifed();
            parentPipe.BuildPipes();
            pipesScript.UpdateEditNodes();
            Debug.Log("Node Added: " + newPipeNode);
        }
    }

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
                Debug.Log("Node Added: " + parentNodeScript);
                renderer.material = selectedMat;
            }
            else
            {
                //ADD WARNING
                Debug.Log("Maximum nodes selected. Cannot add more.");
            }
        }
        if (pipesScript.selectedNodes.Count == 0)
        {
            addButton.enabled = false;
        }
        if (pipesScript.selectedNodes.Count == 1)
        {
            addButton.enabled = true;
        }
        if (pipesScript.selectedNodes.Count == 2)
        {
            addButton.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (grabInteractable.isSelected)
        {
            parentPipe.SetAllModifed();
            parentPipe.BuildPipes();
        }
    }
}
