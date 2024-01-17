using System.Collections;
using System.Collections.Generic;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class WristUI : MonoBehaviour
{
    public InputActionAsset inputActions;

    public GameObject NewPipeLogic;

    private Canvas _wristUICanvas;
    public GameObject UI;
    private InputAction _menu;
    public GlobalVariables globalVariables;

    public Transform controller;
    // Start is called before the first frame update
    private void Start()
    {
        _wristUICanvas = GetComponent<Canvas>();
        _menu = inputActions.FindActionMap("XRI LeftHand").FindAction("Menu");
        _menu.Enable();
        _menu.performed += ToggleMenu;
        UI.transform.SetParent(controller.transform, false);
        Vector3 newPosition = new Vector3(-0.08f, 0.07f, -0.05f); // Replace x, y, z with your desired coordinates
        UI.transform.localPosition = newPosition;
    }

    public void PipeCreation()
    {   
         Instantiate(NewPipeLogic);
    }

    public void AddNode()
    {
        // Cambiar por pipe en edicion activa
        PipesScript pipeBeingEdited = globalVariables.pipeBeingEdited.GetComponent<PipesScript>();
        List<PipeNode> nodesList = pipeBeingEdited.selectedNodes;
        if(nodesList.Count != 1){
            //ADD WARNING
            Debug.Log("Mas de un nodo seleccionado");
        }
        if(nodesList.Count == 0){
            //ADD WARNING (VALIDATION CHANGED TO NODE EDIT MODE LOGIC)
            Debug.Log("Seleccione un nodo");
        }
        else
        {
            // Adds a node next to the selected node
            Debug.Log(nodesList[0]);
            pipeBeingEdited.AddNode();
        }
        // // Deselects node after adding
        // nodesList[0].GetComponentInChildren<NodeEditModeLogic>().SetSelectedNodes();
    }

    public void AddNodeBetween()
    {
        globalVariables.pipeBeingEdited.GetComponent<PipesScript>().InsertNodeInBetween();
    }

    public void DeleteNode()
    {
        globalVariables.pipeBeingEdited.GetComponent<PipesScript>().DeleteNode();
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        _menu.performed += ToggleMenu;
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        _wristUICanvas.enabled = !_wristUICanvas.enabled;
    }
}
