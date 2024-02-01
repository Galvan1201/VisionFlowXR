using System.Collections;
using System.Collections.Generic;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Wave.Essence.ScenePerception.Sample;
using UnityEngine.UI;

public class GlobalVariables : MonoBehaviour
{
    public List<GameObject> pipesOnScene;
    public float budget = 0;
    public float currentCost = 0;
    public float remainingBudget = 0;
    public GameObject pipeBeingEdited;
    public GameObject pipesOnSceneContainer;
    public InputActionAsset inputActions;
    public GameObject mainMenu;
    public AlertHandler alert;
    public int pipesPlacedCount = 1;

    private InputAction _mainMenu;
    private InputAction _meshTransparencyUp;
    private InputAction _meshTransparencyDown;
    public Slider alphaSlider;
    // Start is called before the first frame update
    void Start()
    {
        UpdatePipesOnScene();

        _mainMenu = inputActions.FindActionMap("XRI LeftHand").FindAction("MainMenu");
        _mainMenu.Enable();
        _mainMenu.performed += ToggleMainMenu;

        _meshTransparencyUp = inputActions.FindActionMap("XRI RightHand").FindAction("MeshTransparencyUp");
        _meshTransparencyUp.Enable();
        _meshTransparencyUp.performed += MeshTransparencyUp;
        _meshTransparencyUp.canceled += StopMeshTransparencyUp;

        _meshTransparencyDown = inputActions.FindActionMap("XRI RightHand").FindAction("MeshTransparencyDown");
        _meshTransparencyDown.Enable();
        _meshTransparencyDown.performed += MeshTransparencyDown;
        _meshTransparencyDown.canceled += StopMeshTransparencyDown;
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
            if (pipeBeingEdited != childTransform.gameObject)
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

    float sliderSpeed = 0.7f;
    private bool isIncreasing = false;
    public void MeshTransparencyDown(InputAction.CallbackContext context)
    {
        Debug.Log("Down");
        // Continuously decrease slider value while the button is held down
        isIncreasing = true;
        StartCoroutine(DecreaseSliderValue());
    }

    public void StopMeshTransparencyDown(InputAction.CallbackContext context)
    {
        Debug.Log("Release");
        // Stop the continuous decrease when the button is released
        isIncreasing = false;
        StopCoroutine(DecreaseSliderValue());
    }

    private IEnumerator DecreaseSliderValue()
    {
        while (isIncreasing)
        {
            // Decrease slider value
            alphaSlider.value -= sliderSpeed * Time.deltaTime;

            // You can adjust the yield return null time to control the rate of decrease
            yield return null;
        }
    }

    public void MeshTransparencyUp(InputAction.CallbackContext context)
    {
        Debug.Log("Up");
        // Continuously increase slider value while the button is held down
        isIncreasing = true;    
        StartCoroutine(IncreaseSliderValue());
    }

    public void StopMeshTransparencyUp(InputAction.CallbackContext context)
    {
        Debug.Log("Release");
        // Stop the continuous increase when the button is released
        isIncreasing = false;
        StopCoroutine(IncreaseSliderValue());
    }

    private IEnumerator IncreaseSliderValue()
    {
        while (isIncreasing)
        {
            // Increase slider value
            alphaSlider.value += sliderSpeed * Time.deltaTime;

            // You can adjust the yield return null time to control the rate of increase
            yield return null;
        }
    }
}
