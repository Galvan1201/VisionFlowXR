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


public class NodeEditModeLogic : MonoBehaviour
{
    private PipeBase parentPipe;
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

        addButton = addObject.GetComponent<Button>();
        addButton.onClick.AddListener(AddNode);
    }

    private void AddNode()
    {
        if (parentNodeScript.CanExtendPipes())
        {
            newPipeNode = parentNodeScript.ExtendPipe().GetComponent<PipeNode>();
            // GameObject child = originalGameObject.transform.GetChild(0).gameObject;
            parentPipe.SetAllModifed();
            parentPipe.BuildPipes();
            pipesScript.UpdateEditNodes();
            Debug.Log("Node Added: " + newPipeNode);
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
