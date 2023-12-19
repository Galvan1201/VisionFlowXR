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

    public Transform controller;
    // Start is called before the first frame update
    private void Start()
    {
        _wristUICanvas = GetComponent<Canvas>();
        _menu = inputActions.FindActionMap("XRI LeftHand").FindAction("Menu");
        _menu.Enable();
        _menu.performed += ToggleMenu;
        UI.transform.SetParent(controller.transform, false);
        Vector3 newPosition = new Vector3(-0.04f, 0.06f, 0); // Replace x, y, z with your desired coordinates
        UI.transform.localPosition = newPosition;
    }

    public void PipeCreation()
    {   
        NewPipeLogic = Instantiate(NewPipeLogic);
    }

    public void AddNode()
    {
        List<PipeNode> nodesList = FindAnyObjectByType<PipesScript>().selectedNodes;
        if(nodesList.Count != 1){
            //ADD WARNING
            Debug.Log("Mas de un nodo seleccionado");
        }
        if(nodesList.Count == 0){
            //ADD WARNING
            Debug.Log("Seleccione un nodo");
        }
        else
        {
            Debug.Log(nodesList[0]);
            nodesList[0].GetComponentInChildren<NodeEditModeLogic>().AddNode();
        }
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
