using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class WristUI : MonoBehaviour
{
    public InputActionAsset inputActions;

    public NewPipeLogic NewPipeLogic;

    private Canvas _wristUICanvas;
    public GameObject UI;
    private InputAction _menu;

    private Button button;

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
        if (!NewPipeLogic.enabled)
        {
            Debug.Log(NewPipeLogic.enabled);
            NewPipeLogic.OnEnable();
        }
        else
        {
            NewPipeLogic.OnDisable();
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
