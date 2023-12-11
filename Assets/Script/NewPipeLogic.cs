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
using UnityEngine.Rendering;

public class NewPipeLogic : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject nodeHolo1;
    private GameObject firstNode;

    private Material pipeMaterial;
    private XRGrabInteractable grabInteractable;
    public List<Vector3> nodePositions = new List<Vector3>();

    public GameObject PlacedPipe;

    private Transform content;

    private TextMeshProUGUI menuHeader;
    public InputActionAsset inputActions;

    private Transform MainCamera;

    private PipeBase Pipe;

    private Slider slider;

    enum PipeCreationSteps
    {
        Step1_NewPipeBttn,
        Step2_SetSettings,
        Step3_ReadyToPlace,
        Step4_GrabObject,
        Step5_ConfirmPosition,
        Step6_SpawnNextObject
    }

    PipeCreationSteps currentStep;
    bool stepChange;

    public void OnEnable()
    {
        // Initialize
        UnityEngine.Debug.Log("StartTutorial");
        currentStep = PipeCreationSteps.Step1_NewPipeBttn;
        settingsUI = Instantiate(settingsUI);
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

        if (currentStep == PipeCreationSteps.Step2_SetSettings)
        {
            slider.onValueChanged.AddListener(delegate { ChangeRadiusOnMenu(); });
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
                MainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();

                // Set the position in front of the main camera
                float distanceFromCamera = 2.0f; // Adjust the distance as needed
                Vector3 newPosition = MainCamera.position + MainCamera.forward * distanceFromCamera;

                // Make sure there is a Rigidbody component attached to the settingsUI
                Rigidbody rigidbody = settingsUI.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    // Temporarily set the Rigidbody to kinematic
                    bool wasKinematic = rigidbody.isKinematic;
                    rigidbody.isKinematic = true;

                    // Set the position
                    settingsUI.transform.position = newPosition;

                    // Restore the original kinematic state
                    rigidbody.isKinematic = wasKinematic;
                }
                else
                {
                    // If there is no Rigidbody, just set the position directly
                    settingsUI.transform.position = newPosition;
                }

                UnityEngine.Debug.Log(content.Find("Radius Slider").Find("Slider").GetComponent<Slider>());
                slider = content.Find("Radius Slider").Find("Slider").GetComponent<Slider>();
                currentStep = PipeCreationSteps.Step2_SetSettings;
                break;

            case PipeCreationSteps.Step2_SetSettings:
                UnityEngine.Debug.Log(currentStep);
                UnityEngine.Debug.Log("Save pipe settings and instansiate prefab");
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
                newPosition = new Vector3(0, 2f, -0.836f); // Replace x, y, z with your desired coordinates;
                firstNode.transform.localPosition = newPosition;
                UnityEngine.Debug.Log(firstNode.transform.position);
                grabInteractable = firstNode.GetComponent<XRGrabInteractable>();
                UnityEngine.Debug.Log("Waiting for grab, step4");
                currentStep = PipeCreationSteps.Step4_GrabObject;
                break;

            case PipeCreationSteps.Step4_GrabObject:
                settingsUI.SetActive(false);
                currentStep = PipeCreationSteps.Step5_ConfirmPosition;
                // if(FirstNodeGrabbed)
                // {
                // }
                // Wait for the user to grab and place the object
                // You might use some input events or conditions to detect grabbing
                break;

            case PipeCreationSteps.Step5_ConfirmPosition:
                UnityEngine.Debug.Log("Position confirmed, hide canvas");
                firstNode.GetComponentInChildren<Canvas>().enabled = false;
                // Store the position of the placed object
                nodePositions.Add(firstNode.transform.position);
                // PlacedPipe.SetActive(true);
                // PlacedPipe.transform.position = firstNode.transform.position;
                // PlacedPipe.GetComponent<PipeBase>().Material = pipeMaterial;
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
    private Slider diameter;
    private TextMeshProUGUI radiusText;

    public void ChangeRadiusOnMenu()
    {
        Pipe.PipeSettings.Radius = slider.value / 100 / 4; //a mm/ de diametro a radio
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
        radiusText = content.Find("Radius Slider").Find("Value").GetComponent<TextMeshProUGUI>();
        radiusText.text = slider.value.ToString();
    }

    // Toggles flanges
    public void ToggleFlangesOnMenu()
    {
        Pipe.PipeSettings.FlangeDetail.Flange = content.Find("Flange Toggle").Find("Offset Anchor").GetComponentInChildren<Toggle>().isOn;
        Pipe.SetAllModifed();
        Pipe.BuildPipes();
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
