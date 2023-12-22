using System.Collections;
using System.Collections.Generic;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalVariables : MonoBehaviour
{
    public List<GameObject> pipesOnScene;
    public float budget;
    public GameObject pipeBeingEdited = null;
    public GameObject pipesOnSceneContainer;
    public InputActionAsset inputActions;
    public GameObject mainMenu;
    
    private InputAction _mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        UpdatePipesOnScene();

        _mainMenu = inputActions.FindActionMap("XRI LeftHand").FindAction("MainMenu");
        _mainMenu.Enable();
        _mainMenu.performed += ToggleMainMenu;
    }

    public void UpdatePipesOnScene()
    {
        pipesOnScene.Clear();
        foreach (Transform childTransform in pipesOnSceneContainer.transform)
        {
            // Add each child GameObject to the list
            pipesOnScene.Add(childTransform.gameObject);
        }
    }

    public void ToggleEditPipe()
    {
        foreach (Transform childTransform in pipesOnSceneContainer.transform)
        {
            if(pipeBeingEdited != childTransform.gameObject)
            {
                childTransform.GetComponent<PipesScript>().ToggleEditMode(false);
            }
            else
            {
                childTransform.GetComponent<PipesScript>().ToggleEditMode(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _mainMenu.performed += ToggleMainMenu;
    }

    public void ToggleMainMenu(InputAction.CallbackContext context)
    {
        UpdatePipesOnScene();
        mainMenu.SetActive(!mainMenu.activeSelf);
    }
}
