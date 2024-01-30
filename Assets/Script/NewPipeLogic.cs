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
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;


public class NewPipeLogic : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject nodeHolo1;
    private GameObject firstNode;
    private GameObject firstNodeAccept;
    private GameObject secondNode;
    private GameObject secondNodeAccept;

    private XRGrabInteractable grabInteractable;
    public List<Vector3> nodePositions = new List<Vector3>();

    private WristUI wristUI;
    private Button newpipeButton;

    // pipeInUI to be placed references
    public GameObject pipeToPlace;
    private PipeSettings pipeToPlaceSettings;
    private float radiusToSet;
    private bool flangeToSet = true;
    private Material materialToSet;


    private Transform content;

    private TextMeshProUGUI menuHeader;
    public InputActionAsset inputActions;

    private Transform MainCamera;

    private PipeBase pipeInUI;

    private Slider slider;
    Slider flangeSlider;
    float flangeSliderValue;
    private Toggle toggle;

    private GlobalVariables globalVariables;

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

    public void OnEnable()
    {
        // Initialize
        UnityEngine.Debug.Log("StartTutorial");
        currentStep = PipeCreationSteps.Step1_NewPipeBttn;
        MainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();
        radiusToSet = 30f/1000f/2f;

        // Set the position in front of the main camera
        float distanceFromCamera = 1f; // Adjust the distance as needed
        Vector3 newPosition = MainCamera.position + MainCamera.forward * distanceFromCamera;
        newPosition += new Vector3(0, -0.4f, 0);
        // Calculate the rotation to face the camera only in the y-axis
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(MainCamera.forward.x, 0, MainCamera.forward.z));
        // Instantiate the UI with the calculated position and rotation
        settingsUI = Instantiate(settingsUI, newPosition, newRotation);
        settingsUI.transform.Find("NewPipeCreatorMenu").Find("Header").Find("Close button").GetComponent<Button>().onClick.AddListener(Close);
        wristUI = FindAnyObjectByType<WristUI>();
        newpipeButton = wristUI.gameObject.transform.Find("Place Pipe").GetComponentInChildren<Button>();
        newpipeButton.interactable = false;

        //Find menu content and UIPipe
        content = settingsUI.transform.Find("NewPipeCreatorMenu").Find("Content");
        pipeInUI = settingsUI.transform.Find("NewPipeCreatorMenu").Find("NewPipeShow").GetComponent<PipeBase>();
        pipeInUI.BuildPipes();
        PerformStep();
        
    }
    void Update()
    {

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
            if (!grabInteractable.isSelected && !firstNodeAccept.activeSelf)
            {
                firstNodeAccept.SetActive(true);
            }

            //if grabbed hide confirm 
            if (grabInteractable.isSelected && firstNodeAccept.activeSelf)
            {   
                firstNodeAccept.SetActive(false);
            }
        }

        if (currentStep == PipeCreationSteps.Step6_ConfirmNode2)
        {
            //Show confirm
            if (!grabInteractable.isSelected && !secondNodeAccept.activeSelf)
            {
                secondNodeAccept.SetActive(true);
            }

            //if grabbed hide confirm 
            if (grabInteractable.isSelected && secondNodeAccept.activeSelf)
            {
                secondNodeAccept.SetActive(false);
            }
        }
    }

    void Close()
    {
        newpipeButton.interactable = true;
        Destroy(settingsUI);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all listeners to prevent memory leaks
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(ChangeRadiusOnMenu);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(ToggleFlangesOnMenu);
        }

        if (firstNodeAccept != null)
        {
            firstNodeAccept.GetComponent<Button>().onClick.RemoveListener(PerformStep);
        }

        if (secondNodeAccept != null)
        {
            secondNodeAccept.GetComponent<Button>().onClick.RemoveListener(PerformStep);
        }

        // Add additional cleanup for other listeners if needed
    }

    public void PerformStep()
    {
        // Check current step and perform actions accordingly
        switch (currentStep)
        {
            case PipeCreationSteps.Step1_NewPipeBttn:
                // UnityEngine.Debug.Log("Step 1: OpenCanvas");
                settingsUI.SetActive(true);

                //Add listeners of Sliders and toggles
                slider = content.Find("Radius Slider").Find("Slider").GetComponent<Slider>();
                slider.onValueChanged.AddListener(ChangeRadiusOnMenu);
                toggle = content.Find("Flange Toggle").Find("Offset Anchor").GetComponentInChildren<Toggle>();
                toggle.onValueChanged.AddListener(ToggleFlangesOnMenu);

                flangeSlider = content.Find("Flange Size").Find("Slider").GetComponent<Slider>();

                TextMeshProUGUI flangeLengthText = content.Find("Flange Size").Find("Value").GetComponent<TextMeshProUGUI>();
                flangeSlider.onValueChanged.AddListener(value => flangeLengthText.text = value.ToString("F2"));

                

                currentStep = PipeCreationSteps.Step2_SetSettings;
                content.Find("Continue button").GetComponent<Button>().onClick.AddListener(PerformStep);
                break;

            case PipeCreationSteps.Step2_SetSettings:
                // UnityEngine.Debug.Log(currentStep);
                // UnityEngine.Debug.Log("Save pipe settings and instansiate prefab");
                flangeSliderValue = content.Find("Flange Size").Find("Slider").GetComponent<Slider>().value;
                content.gameObject.SetActive(false);
                pipeInUI.gameObject.SetActive(false);
                currentStep = PipeCreationSteps.Step3_ReadyToPlace;
                PerformStep();
                break;

            case PipeCreationSteps.Step3_ReadyToPlace:
                menuHeader = settingsUI.transform.Find("NewPipeCreatorMenu").Find("Header").GetComponent<TextMeshProUGUI>();
                menuHeader.SetText("Set the starting point of your pipe");
                // Spawn the node
                firstNode = Instantiate(nodeHolo1);
                firstNodeAccept = firstNode.transform.Find("Accept").gameObject;
                // UnityEngine.Debug.Log(firstNode.transform.position);
                firstNode.transform.SetParent(settingsUI.transform);
                Vector3 newPosition = new Vector3(0, 2f, -0.836f); // Replace x, y, z with your desired coordinates;
                firstNode.transform.localPosition = newPosition;
                // UnityEngine.Debug.Log(firstNode.transform.position);
                grabInteractable = firstNode.GetComponent<XRGrabInteractable>();
                // UnityEngine.Debug.Log("Waiting for grab, step4");
                currentStep = PipeCreationSteps.Step4_GrabObject;
                break;

            case PipeCreationSteps.Step4_GrabObject:
                Destroy(settingsUI);
                globalVariables = GameObject.Find("GlobalVariables").GetComponent<GlobalVariables>();
                globalVariables.pipeBeingEdited = null;
                globalVariables.ToggleEditPipe();
                currentStep = PipeCreationSteps.Step5_ConfirmPosition;
                firstNodeAccept.GetComponent<Button>().onClick.AddListener(PerformStep);

                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_ConfirmPosition:
                // UnityEngine.Debug.Log("Position confirmed, hide canvas");
                firstNodeAccept.SetActive(false);
                firstNode.transform.parent = null;
                grabInteractable.enabled = false;
                // Store the position of the first node on the list
                nodePositions.Add(firstNode.transform.position);
                newPosition = new Vector3(0, 0.1f, 0.1f) + firstNode.transform.position; // Replace x, y, z with your desired coordinates;
                secondNode = Instantiate(nodeHolo1, newPosition, Quaternion.identity);
                grabInteractable = secondNode.GetComponent<XRGrabInteractable>();
                secondNodeAccept = secondNode.transform.Find("Accept").gameObject;
                secondNodeAccept.SetActive(true);
                currentStep = PipeCreationSteps.Step6_ConfirmNode2;
                secondNodeAccept.GetComponent<Button>().onClick.AddListener(PerformStep);
                break;

            case PipeCreationSteps.Step6_ConfirmNode2:
                // Store the position of the first node on the list
                nodePositions.Add(secondNode.transform.position);
                Destroy(firstNode);
                Destroy(secondNode);
                //Places the pipe and sets its components
                pipeToPlace = Instantiate(pipeToPlace);
                pipeToPlaceSettings = pipeToPlace.GetComponent<PipeBase>().PipeSettings;
                pipeToPlaceSettings.Radius = radiusToSet;
                PipesScript pipeToPlaceScript = pipeToPlace.GetComponent<PipesScript>();
                pipeToPlaceScript.radius = radiusToSet;
                pipeToPlaceSettings.FlangeDetail.Flange = flangeToSet;
                pipeToPlaceSettings.FlangeDetail.Size = radiusToSet / 3;
                pipeToPlaceSettings.FlangeDetail.Length = radiusToSet * 2;
                pipeToPlaceSettings.CornersDetail.Size = radiusToSet;
                pipeToPlaceSettings.CornersDetail.Steps = 6;

                //Intervalo a 1 metro, cambiar.
                pipeToPlaceSettings.FlangeDetail.Interval = flangeSliderValue;
                //pipeToPlaceSettings.FlangeDetail.Interval = 1;
                if (materialToSet != null)
                {
                    pipeToPlace.GetComponent<PipeBase>().Material = materialToSet;
                }
                else
                {
                    // Set a default value if materialToSet is null
                    pipeToPlace.GetComponent<PipeBase>().Material = pipeInUI.Material; // replace defaultMaterial with the desired default material
                }
                //Sends the nodesList to the custom pipe builder
                pipeToPlaceScript.nodesPositions = nodePositions;
                pipeToPlace.name = "Pipe No." + globalVariables.pipesPlacedCount.ToString();
                globalVariables.pipesPlacedCount += 1;
                pipeToPlaceScript.CreateInitialPipeFromList();
                pipeToPlace.transform.SetParent(GameObject.Find("Pipes").transform);
                pipeToPlace.SetActive(true);
                newpipeButton.interactable = true;
                Destroy(gameObject);
                break;

            // Add more steps as needed

            default:
                break;
        }
    }

    // Change radius of pipe with slider
    private TextMeshProUGUI radiusText;

    public void ChangeRadiusOnMenu(float radius)
    {
        pipeInUI.PipeSettings.Radius = slider.value / 300 / 5/2; //a mm/ de diametro a radio
        pipeInUI.SetAllModifed();
        pipeInUI.BuildPipes();
        radiusText = content.Find("Radius Slider").Find("Value").GetComponent<TextMeshProUGUI>();
        radiusText.text = slider.value.ToString();
        radiusToSet = radius / 1000/2;
    }

    // Toggles flanges
    public void ToggleFlangesOnMenu(bool state)
    {
        pipeInUI.PipeSettings.FlangeDetail.Flange = state;
        pipeInUI.SetAllModifed();
        pipeInUI.BuildPipes();
        flangeToSet = state;
    }

    // Change material
    private TextMeshProUGUI materialText;

    public void ChangeMaterial(Renderer rendererUI)
    {
        materialToSet = rendererUI.material;
        pipeInUI.Material = rendererUI.material;
        materialText = content.Find("Materials").Find("Scroll UI Sample").Find("MatName").GetComponent<TextMeshProUGUI>();
        materialText.text = rendererUI.material.name.Replace(" (Instance)", "");
        pipeInUI.SetAllModifed();
        pipeInUI.BuildPipes();
    }

    // void SpawnGrabbableObject()
    // {
    //     // Instantiate the grabbable object at the spawn point
    //     GameObject newObject = Instantiate(nodeHolo, spawnPoint.position, Quaternion.identity);
    //     // You might want to configure the object or add components here
    // }
}
