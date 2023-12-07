using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using eWolf.PipeBuilder.Helpers;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder;
using Unity.XR.CoreUtils;
using TMPro;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.Rendering;

public class NewPipeLogic : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject nodeHolo1;
    private GameObject firstNode;

    private Material pipeMaterial;
    private XRGrabInteractable grabInteractable;
    public Transform spawnPoint;
    public List<Vector3> nodePositions = new List<Vector3>();

    public GameObject PlacedPipe;
    public GameObject creatorMenuHeader;
    private TextMeshProUGUI menuHeader;
    public InputActionAsset inputActions;

    public Transform MainCamera;

    public PipeBase Pipe;

    enum PipeCreationSteps
    {
        Step1_NewPipeBttn,
        Step2_SetSettings,
        Step3_ReadyToPlace,
        Step4_GrabObject,
        Step5_StorePosition,
        Step6_SpawnNextObject
    }

    PipeCreationSteps currentStep;
    bool stepChange;

    public void OnEnable()
    {
        // Initialize
        UnityEngine.Debug.Log("StartTutorial");
        currentStep = PipeCreationSteps.Step1_NewPipeBttn;
        settingsUI.SetActive(false);
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

        if (currentStep == PipeCreationSteps.Step5_StorePosition)
        {
            if (!grabInteractable.isSelected)
            {
                PerformStep();
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
                settingsUI.transform.position = MainCamera.transform.position;
                Vector3 newPosition = new Vector3(0.023f, 1.256f, 0.885f); // Replace x, y, z with your desired coordinates
                settingsUI.transform.position = newPosition;
                currentStep = PipeCreationSteps.Step2_SetSettings;
                break;

            case PipeCreationSteps.Step2_SetSettings:
                UnityEngine.Debug.Log(currentStep);
                UnityEngine.Debug.Log("Save pipe settings and instansiate prefab");
                currentStep = PipeCreationSteps.Step3_ReadyToPlace;
                PerformStep();
                break;

            case PipeCreationSteps.Step3_ReadyToPlace:
                menuHeader = creatorMenuHeader.GetComponentInChildren<TextMeshProUGUI>();
                menuHeader.SetText("Set the starting point of your pipe");
                // Spawn the node
                firstNode = Instantiate(nodeHolo1);
                UnityEngine.Debug.Log(firstNode.transform.position);
                firstNode.transform.SetParent(settingsUI.transform);
                newPosition = new Vector3(0, 2f, -0.836f); // Replace x, y, z with your desired coordinates;
                firstNode.transform.localPosition = newPosition;
                UnityEngine.Debug.Log(firstNode.transform.position);
                grabInteractable = firstNode.GetComponent<XRGrabInteractable>();
                UnityEngine.Debug.Log("Waiting for grab, step4");
                currentStep = PipeCreationSteps.Step4_GrabObject;
                break;

            case PipeCreationSteps.Step4_GrabObject:
                settingsUI.SetActive(false);
                currentStep = PipeCreationSteps.Step5_StorePosition;
                // if(FirstNodeGrabbed)
                // {
                // }
                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_StorePosition:
                UnityEngine.Debug.Log("Pelota suelta");
                // Store the position of the placed object
                nodePositions.Add(firstNode.transform.position);
                PlacedPipe.SetActive(true);
                PlacedPipe.transform.position = firstNode.transform.position;
                PlacedPipe.GetComponent<PipeBase>().Material = pipeMaterial;
                currentStep = PipeCreationSteps.Step6_SpawnNextObject;
                break;

            case PipeCreationSteps.Step6_SpawnNextObject:
                // Spawn the next object next to the previously placed object
                if (Input.GetKeyDown(KeyCode.Space)) // Replace with your condition to spawn next object
                {
                    // SpawnGrabbableObject();
                    currentStep = PipeCreationSteps.Step4_GrabObject; // Repeat the process for the next object
                }
                break;

            // Add more steps as needed

            default:
                break;
        }
    }

    // Change radius of pipe with slider
    public GameObject sliderParent;
    private Slider diameter;
    private TextMeshProUGUI radiusText;

    public void ChangeRadiusOnMenu()
    {
        diameter = sliderParent.GetComponentInChildren<Slider>();
        Pipe.PipeSettings.Radius = diameter.value / 100 / 4; //a mm/ de diametro a radio
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
        radiusText = sliderParent.GetComponentInChildren<TextMeshProUGUI>();
        radiusText.text = diameter.value.ToString();
    }

    // Toggles flanges
    public Toggle toggleFlanges;
    public void ToggleFlangesOnMenu()
    {
        Pipe.PipeSettings.FlangeDetail.Flange = toggleFlanges.isOn;
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
    }

    // Change material
    public GameObject materialCube;
    public GameObject materialHeader;
    private TextMeshProUGUI materialText;

    public void ChangeMaterial(Renderer rendererUI)
    {
        pipeMaterial = rendererUI.material;
        Pipe.Material = rendererUI.material;
        materialText = materialHeader.GetComponentInChildren<TextMeshProUGUI>();
        materialText.text = rendererUI.material.name.Replace(" (Instance)", "");
    }

    // void SpawnGrabbableObject()
    // {
    //     // Instantiate the grabbable object at the spawn point
    //     GameObject newObject = Instantiate(nodeHolo, spawnPoint.position, Quaternion.identity);
    //     // You might want to configure the object or add components here
    // }
}
