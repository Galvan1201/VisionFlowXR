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

    public void OnEnable()
    {
        Debug.Log("Entra a OnEnable");
        // Initialize
        UnityEngine.Debug.Log("StartTutorial");
        currentStep = PipeCreationSteps.Step1_NewPipeBttn;
        MainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();
        radiusToSet = 30f/1000f;

        // Set the position in front of the main camera
        float distanceFromCamera = 1f; // Adjust the distance as needed
        Vector3 newPosition = MainCamera.position + MainCamera.forward * distanceFromCamera;
        newPosition += new Vector3(0, -0.4f, 0);
        // Calculate the rotation to face the camera only in the y-axis
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(MainCamera.forward.x, 0, MainCamera.forward.z));
        // Instantiate the UI with the calculated position and rotation
        settingsUI = Instantiate(settingsUI, newPosition, newRotation);
        wristUI = FindAnyObjectByType<WristUI>();
        newpipeButton = wristUI.gameObject.transform.Find("Place Pipe").GetComponent<Button>();
        newpipeButton.enabled = false;

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

    public void OnDisable()
    {
        // settingsUI.SetActive(false);
    }

    public void PerformStep()
    {
        // Check current step and perform actions accordingly
        switch (currentStep)
        {
            case PipeCreationSteps.Step1_NewPipeBttn:
                UnityEngine.Debug.Log("Step 1: OpenCanvas");
                settingsUI.SetActive(true);

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
                Destroy(settingsUI);
                currentStep = PipeCreationSteps.Step5_ConfirmPosition;
                firstNodeAccept.GetComponent<Button>().onClick.AddListener(PerformStep);

                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_ConfirmPosition:
                UnityEngine.Debug.Log("Position confirmed, hide canvas");
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
                pipeToPlaceSettings.FlangeDetail.Interval = 1;
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
                pipeToPlaceScript.CreateInitialPipeFromList();
                pipeToPlace.SetActive(true);
                newpipeButton.enabled = true;
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
        pipeInUI.PipeSettings.Radius = slider.value / 100 / 4; //a mm/ de diametro a radio
        pipeInUI.SetAllModifed();
        pipeInUI.BuildPipes();
        radiusText = content.Find("Radius Slider").Find("Value").GetComponent<TextMeshProUGUI>();
        radiusText.text = slider.value.ToString();
        radiusToSet = radius / 1000;
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
