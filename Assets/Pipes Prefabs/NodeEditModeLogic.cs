using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Animations;


public class NodeEditModeLogic : MonoBehaviour
{   
    private PipeBase parentPipe;
    private GameObject addObject;
    private GameObject removeObject;
    private PipesScript pipesScript;
    private XRGrabInteractable grabInteractable;

    // Start is called before the first frame update
    void OnEnable()
    {
        parentPipe = transform.parent.GetComponentInParent<PipeBase>();
        Debug.Log(parentPipe);
        addObject = transform.Find("Editables").Find("Add").gameObject;
        removeObject = transform.Find("Editables").Find("Remove").gameObject;
        pipesScript = transform.parent.GetComponentInParent<PipesScript>(); 
        Debug.Log(pipesScript); 
        grabInteractable = gameObject.GetComponent<XRGrabInteractable>();
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
