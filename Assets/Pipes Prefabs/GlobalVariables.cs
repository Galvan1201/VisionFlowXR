using System.Collections;
using System.Collections.Generic;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Wave.Essence.ScenePerception.Sample;

public class GlobalVariables : MonoBehaviour
{
    public List<GameObject> pipesOnScene;
    public float budget=0;
    public float currentCost=0;
    public float remainingBudget=0;
    public GameObject pipeBeingEdited;
    public GameObject pipesOnSceneContainer;
    public InputActionAsset inputActions;
    public GameObject mainMenu;
    public AlertHandler alert;
    public int pipesPlacedCount = 1;
    
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
                childTransform.GetComponent<PipeBase>().Material = childTransform.GetComponentInChildren<NodeEditModeLogic>()?.originalMaterial ?? childTransform.GetComponent<PipeBase>()?.Material;
            }
            else
            {
                childTransform.GetComponent<PipesScript>().ToggleEditMode(true);
            }
        }
    }

    public void budgetUpdate()
    {   
        currentCost = 0;
        foreach (Transform childTransform in pipesOnSceneContainer.transform)
        {
            childTransform.GetComponent<PipesScript>().CalculateCost();
            currentCost += childTransform.GetComponent<PipesScript>().totalCost;
        }
        remainingBudget = budget - currentCost;
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
