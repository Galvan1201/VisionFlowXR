using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eWolf.PipeBuilder.Helpers;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder;
using Unity.XR.CoreUtils;
using TMPro;
using System.Diagnostics;
using UnityEditor;

public class NewPipeLogic : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject grabbablePrefab;
    public Transform spawnPoint;
    public List<Vector3> nodePositions = new List<Vector3>();

    public GameObject creatorMenuHeader;
    private TextMeshProUGUI menuHeader;

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
        // Instantiate(settingsUI);
        settingsUI.SetActive(false);
        PerformStep();
    }

    private void Update()
    {
        if (stepChange)
        {
            stepChange = false;
            PerformStep();
        }
    }

    public void OnDisable(){
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
                menuHeader = creatorMenuHeader.GetComponentInChildren<TextMeshProUGUI>();
                menuHeader.SetText("Prueba123");
                UnityEngine.Debug.Log(currentStep);
                break;

            case PipeCreationSteps.Step3_ReadyToPlace:
                // Handle UI button click to start the grabbing process
                if (Input.GetKeyDown(KeyCode.Space)) // Replace with your UI button click event
                {
                    settingsUI.SetActive(false);
                    SpawnGrabbableObject();
                    currentStep = PipeCreationSteps.Step4_GrabObject;
                    stepChange = true;
                }
                break;

            case PipeCreationSteps.Step4_GrabObject:
                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_StorePosition:
                // Store the position of the placed object
                nodePositions.Add(grabbablePrefab.transform.position);
                currentStep = PipeCreationSteps.Step6_SpawnNextObject;
                break;

            case PipeCreationSteps.Step6_SpawnNextObject:
                // Spawn the next object next to the previously placed object
                if (Input.GetKeyDown(KeyCode.Space)) // Replace with your condition to spawn next object
                {
                    SpawnGrabbableObject();
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
        Pipe.Material = rendererUI.material;
        materialText = materialHeader.GetComponentInChildren<TextMeshProUGUI>();
        materialText.text = rendererUI.material.name.Replace(" (Instance)","");
    }   

    void SpawnGrabbableObject()
    {
        // Instantiate the grabbable object at the spawn point
        GameObject newObject = Instantiate(grabbablePrefab, spawnPoint.position, Quaternion.identity);
        // You might want to configure the object or add components here
    }
}
