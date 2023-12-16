using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using eWolf.PipeBuilder.Data;


public class NewPipeLogic : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject nodeHolo1;
    private GameObject firstNode;
    private GameObject secondNode;

    private Material pipeMaterial;
    private XRGrabInteractable grabInteractable;
    public List<Vector3> nodePositions = new List<Vector3>();

    public GameObject pipeToPlace;
    private PipeSettings pipeToPlaceSettings;
    private float radiusToSet;
    private bool flangeToSet;


    private Transform content;

    private TextMeshProUGUI menuHeader;
    public InputActionAsset inputActions;

    private Transform MainCamera;

    private PipeBase Pipe;

    private Slider slider;
    private Toggle toggle;

    enum PipeCreationSteps
    {
        Step1_NewPipeBttn,
        Step2_SetSettings,
        Step3_ReadyToPlace,
        Step4_GrabObject,
        Step5_ConfirmPosition,
        Step6_ConfirmNode2
    }

    PipeCreationSteps currentStep;
    bool stepChange;

    public void OnEnable()
    {
        // Initialize
        UnityEngine.Debug.Log("StartTutorial");
        currentStep = PipeCreationSteps.Step1_NewPipeBttn;
        MainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();

        // Set the position in front of the main camera
        float distanceFromCamera = 1f; // Adjust the distance as needed
        Vector3 newPosition = MainCamera.position + MainCamera.forward * distanceFromCamera;
        newPosition += new Vector3(0, -0.4f, 0);
        // Calculate the rotation to face the camera only in the y-axis
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(MainCamera.forward.x, 0, MainCamera.forward.z));
        // Instantiate the UI with the calculated position and rotation
        settingsUI = Instantiate(settingsUI, newPosition, newRotation);

        //Find menu content and UIPipe
        content = settingsUI.transform.Find("NewPipeCreatorMenu").Find("Content");
        Pipe = settingsUI.transform.Find("NewPipeCreatorMenu").Find("NewPipeShow").GetComponent<PipeBase>();
        PerformStep();
    }
    void Update()
    {
        if (stepChange)
        {
            UnityEngine.Debug.Log(stepChange);
            stepChange = false;
            PerformStep();
        }

        if (currentStep == PipeCreationSteps.Step4_GrabObject)
        {
            if (grabInteractable.isSelected)
            {
                PerformStep();
            }
        }

        if (currentStep == PipeCreationSteps.Step5_ConfirmPosition)
        {
            // UnityEngine.Debug.Log(firstNode.GetComponentInChildren<Canvas>().enabled);
            //Show confirm
            if (!grabInteractable.isSelected && !firstNode.GetComponentInChildren<Canvas>().enabled)
            {
                firstNode.GetComponentInChildren<Canvas>().enabled = true;
                UnityEngine.Debug.Log("mostrar canvas");
            }

            //if grabbed hide confirm 
            if (grabInteractable.isSelected && firstNode.GetComponentInChildren<Canvas>().enabled)
            {
                firstNode.GetComponentInChildren<Canvas>().enabled = false;
                UnityEngine.Debug.Log("grabbing, hide canvas");
            }
        }

        if (currentStep == PipeCreationSteps.Step6_ConfirmNode2)
        {
            // UnityEngine.Debug.Log(firstNode.GetComponentInChildren<Canvas>().enabled);
            //Show confirm
            if (!grabInteractable.isSelected && !secondNode.GetComponentInChildren<Canvas>().enabled)
            {
                firstNode.GetComponentInChildren<Canvas>().enabled = true;
                UnityEngine.Debug.Log("mostrar canvas");
            }

            //if grabbed hide confirm 
            if (grabInteractable.isSelected && secondNode.GetComponentInChildren<Canvas>().enabled)
            {
                firstNode.GetComponentInChildren<Canvas>().enabled = false;
                UnityEngine.Debug.Log("grabbing, hide canvas");
            }
        }
    }

    public void OnDisable()
    {
        settingsUI.SetActive(false);
    }

    public void PerformStep()
    {
        // Check current step and perform actions accordingly
        switch (currentStep)
        {
            case PipeCreationSteps.Step1_NewPipeBttn:
                UnityEngine.Debug.Log("Step 1: OpenCanvas");
                settingsUI.SetActive(true);
                UnityEngine.Debug.Log(content.Find("Radius Slider").Find("Slider").GetComponent<Slider>());

                //Add listeners of Sliders and toggles
                slider = content.Find("Radius Slider").Find("Slider").GetComponent<Slider>();
                slider.onValueChanged.AddListener(ChangeRadiusOnMenu);
                toggle = content.Find("Flange Toggle").Find("Offset Anchor").GetComponentInChildren<Toggle>();
                toggle.onValueChanged.AddListener(ToggleFlangesOnMenu);

                currentStep = PipeCreationSteps.Step2_SetSettings;
                content.Find("Continue button").GetComponent<Button>().onClick.AddListener(PerformStep);
                break;

            case PipeCreationSteps.Step2_SetSettings:
                UnityEngine.Debug.Log(currentStep);
                UnityEngine.Debug.Log("Save pipe settings and instansiate prefab");
                content.gameObject.SetActive(false);
                Pipe.gameObject.SetActive(false);
                currentStep = PipeCreationSteps.Step3_ReadyToPlace;
                PerformStep();
                break;

            case PipeCreationSteps.Step3_ReadyToPlace:
                menuHeader = settingsUI.transform.Find("NewPipeCreatorMenu").Find("Header").GetComponent<TextMeshProUGUI>();
                menuHeader.SetText("Set the starting point of your pipe");
                // Spawn the node
                firstNode = Instantiate(nodeHolo1);
                UnityEngine.Debug.Log(firstNode.transform.position);
                firstNode.transform.SetParent(settingsUI.transform);
                Vector3 newPosition = new Vector3(0, 2f, -0.836f); // Replace x, y, z with your desired coordinates;
                firstNode.transform.localPosition = newPosition;
                UnityEngine.Debug.Log(firstNode.transform.position);
                grabInteractable = firstNode.GetComponent<XRGrabInteractable>();
                UnityEngine.Debug.Log("Waiting for grab, step4");
                currentStep = PipeCreationSteps.Step4_GrabObject;
                break;

            case PipeCreationSteps.Step4_GrabObject:
                settingsUI.SetActive(false);
                currentStep = PipeCreationSteps.Step5_ConfirmPosition;
                firstNode.transform.Find("Canvas").Find("Accept").GetComponent<Button>().onClick.AddListener(PerformStep);
                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_ConfirmPosition:
                UnityEngine.Debug.Log("Position confirmed, hide canvas");
                firstNode.GetComponentInChildren<Canvas>().enabled = false;
                firstNode.transform.parent = null;
                grabInteractable.enabled = false;
                // Store the position of the first node on the list
                nodePositions.Add(firstNode.transform.position);
                newPosition = new Vector3(0, 0.1f, 0.1f) + firstNode.transform.position; // Replace x, y, z with your desired coordinates;
                secondNode = Instantiate(nodeHolo1, newPosition, Quaternion.identity);
                secondNode.GetComponentInChildren<Canvas>().enabled = true;
                currentStep = PipeCreationSteps.Step6_ConfirmNode2;
                secondNode.transform.Find("Canvas").Find("Accept").GetComponent<Button>().onClick.AddListener(PerformStep);
                break;

            case PipeCreationSteps.Step6_ConfirmNode2:
                // Store the position of the first node on the list
                nodePositions.Add(secondNode.transform.position);
                firstNode.SetActive(false);
                secondNode.SetActive(false);
                //Places the pipe and sets its components
                pipeToPlace = Instantiate(pipeToPlace);
                pipeToPlaceSettings = pipeToPlace.GetComponent<PipeBase>().PipeSettings;
                pipeToPlaceSettings.Radius = radiusToSet;
                pipeToPlaceSettings.FlangeDetail.Flange = flangeToSet;
                pipeToPlace.GetComponent<PipeBase>().Material = pipeMaterial;
                //Sends the nodesList to the custom pipe builder
                PipesScript pipesScript = pipeToPlace.GetComponent<PipesScript>();
                pipesScript.nodesPositions = nodePositions;
                pipesScript.CreateBasicPipeList();
                pipeToPlace.SetActive(true);
                break;

            // Add more steps as needed

            default:
                break;
        }
    }

    // Change radius of pipe with slider
    private Slider diameter;
    private TextMeshProUGUI radiusText;

    public void ChangeRadiusOnMenu(float radius)
    {
        Pipe.PipeSettings.Radius = slider.value / 100 / 4; //a mm/ de diametro a radio
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
        radiusText = content.Find("Radius Slider").Find("Value").GetComponent<TextMeshProUGUI>();
        radiusText.text = slider.value.ToString();
        radiusToSet = radius / 1000;
    }

    // Toggles flanges
    public void ToggleFlangesOnMenu(bool state)
    {
        Pipe.PipeSettings.FlangeDetail.Flange = state;
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
        flangeToSet = state;
    }

    // Change material
    private TextMeshProUGUI materialText;

    public void ChangeMaterial(Renderer rendererUI)
    {
        pipeMaterial = rendererUI.material;
        Pipe.Material = rendererUI.material;
        materialText = content.Find("Materials").Find("Scroll UI Sample").Find("MatName").GetComponent<TextMeshProUGUI>();
        materialText.text = rendererUI.material.name.Replace(" (Instance)", "");
    }

    // void SpawnGrabbableObject()
    // {
    //     // Instantiate the grabbable object at the spawn point
    //     GameObject newObject = Instantiate(nodeHolo, spawnPoint.position, Quaternion.identity);
    //     // You might want to configure the object or add components here
    // }
}
