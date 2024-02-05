using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using eWolf.PipeBuilder;
using eWolf.PipeBuilder.VisionFlowScripts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using eWolf.PipeBuilder.Data;

public class MainMenu : MonoBehaviour
{
    public WarningChooseHandler warningDialog;
    public GameObject pipesPanel;
    public GameObject pipesPanelScreen;
    public GameObject appSettingsPanel;
    public GlobalVariables globalVariables;
    [SerializeField] private GameObject pipeElementInList;

    // Pipe Settings refferences
    private PipeSettings pipeToModifySettings;
    public GameObject returnButton;
    public GameObject pipeSettings;
    TextMeshProUGUI settingsCostText;
    TextMeshProUGUI settingsLengthText;
    TextMeshProUGUI settingsPipeName;
    Slider settingsDiameter;
    TextMeshProUGUI settingsDiameterText;
    Toggle settingsFlangesSet;
    Slider settingsFlangeLength;
    TextMeshProUGUI settingsFlangeLengthText;
    Material settingsMaterial;
    TextMeshProUGUI settingsMaterialText;
    Button applyButton;


    void Awake()
    {
        gameObject.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Return);

        settingsCostText = pipeSettings.transform.Find("costValue").GetComponent<TextMeshProUGUI>();
        settingsLengthText = pipeSettings.transform.Find("lengthValue").GetComponent<TextMeshProUGUI>();

        settingsPipeName = pipeSettings.transform.Find("PipeName").GetComponent<TextMeshProUGUI>();

        settingsDiameter = pipeSettings.transform.Find("Diameter").Find("Slider").GetComponent<Slider>();
        settingsDiameterText = pipeSettings.transform.Find("Diameter").Find("diameterValue").GetComponent<TextMeshProUGUI>();
        settingsDiameter.onValueChanged.AddListener(value => settingsDiameterText.text = value.ToString());

        settingsFlangesSet = pipeSettings.transform.Find("FlangeOn").Find("Toggle").GetComponentInChildren<Toggle>();
        settingsFlangeLength = pipeSettings.transform.Find("FlangeSectionLength").Find("Slider").GetComponent<Slider>();
        settingsFlangeLengthText = pipeSettings.transform.Find("FlangeSectionLength").Find("flangeValue").GetComponent<TextMeshProUGUI>();
        settingsFlangeLength.onValueChanged.AddListener(value => settingsFlangeLengthText.text = value.ToString("F2"));

        settingsMaterialText = pipeSettings.transform.Find("Materials").Find("Scroll UI Sample").Find("MatName").GetComponent<TextMeshProUGUI>();

        applyButton = pipeSettings.transform.Find("Apply").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateTable();
    }

    public void UpdateTable()
    {
        foreach (Transform child in pipesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject pipe in globalVariables.pipesOnScene)
        {
            GameObject generatedElement = Instantiate(pipeElementInList, pipesPanel.transform);
            // generatedElement.transform.Find("costValue").GetComponent<TextMeshProUGUI>().text = pipe.GetComponent<PipesScript>().cost.ToString();
            generatedElement.name = pipe.name;
            generatedElement.transform.Find("PipeName").GetComponent<TextMeshProUGUI>().text = pipe.name;
            generatedElement.transform.Find("costValue").GetComponent<TextMeshProUGUI>().text = "12345";
            pipe.GetComponent<PipesScript>().CalculateTotalLength();
            generatedElement.transform.Find("lengthValue").GetComponent<TextMeshProUGUI>().text = pipe.GetComponent<PipesScript>().totalLength.ToString("F2");
            pipe.GetComponent<PipesScript>().CalculateCost();
            generatedElement.transform.Find("costValue").GetComponent<TextMeshProUGUI>().text = pipe.GetComponent<PipesScript>().totalCost.ToString("F2");
            generatedElement.transform.Find("Configure").GetComponent<Button>().onClick.AddListener(() => PipeSettings(pipe));
            generatedElement.transform.Find("Edit").GetComponent<Button>().onClick.AddListener(() => EditPipeScreen(pipe, generatedElement));
            generatedElement.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(() => DeletePipe(pipe));
            if (globalVariables.pipeBeingEdited == pipe)
            {
                generatedElement.GetComponent<Image>().color = new Color(132f / 255f, 231f / 255f, 213f / 255f, 1f); // This is the color #F7B142;
            }
        }
        gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(Close);
    }

    void OnDisable()
    {
        applyButton.onClick.RemoveAllListeners();
        pipesPanelScreen.SetActive(true);
        pipeSettings.SetActive(false);
        returnButton.SetActive(false);
        appSettingsPanel.SetActive(false);
    }

    void Close()
    {
        applyButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    void PipeSettings(GameObject pipe)
    {
        //Read the pipe settings
        pipesPanelScreen.SetActive(false);
        pipeSettings.SetActive(true);
        returnButton.SetActive(true);
        appSettingsPanel.SetActive(false);

        settingsPipeName.text = pipe.name;
        pipe.GetComponent<PipesScript>().CalculateCost();
        settingsCostText.text = pipe.GetComponent<PipesScript>().totalCost.ToString("F2");

        pipe.GetComponent<PipesScript>().CalculateTotalLength();
        settingsLengthText.text = pipe.GetComponent<PipesScript>().totalLength.ToString("F2");

        settingsDiameter.value = pipe.GetComponent<PipesScript>().radius * 2 * 1000;
        settingsDiameterText.text = settingsDiameter.value.ToString();

        settingsFlangesSet.isOn = pipe.GetComponent<PipeBase>().PipeSettings.FlangeDetail.Flange;
        settingsFlangeLength.value = pipe.GetComponent<PipeBase>().PipeSettings.FlangeDetail.Interval;
        settingsFlangeLengthText.text = settingsFlangeLength.value.ToString("F2");

        settingsMaterial = pipe.GetComponent<PipeBase>().Material;
        settingsMaterialText.text = settingsMaterial.name.Replace(" (Instance)", "");

        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(() => ApplyChanges(pipe));
    }

    public void AppSettings()
    {
        pipesPanelScreen.SetActive(false);
        pipeSettings.SetActive(false);
        returnButton.SetActive(false);
        appSettingsPanel.SetActive(true);
        applyButton.onClick.RemoveAllListeners();
    }


    public void ChangeMaterial(Renderer rendererUI)
    {
        settingsMaterial = rendererUI.material;
        settingsMaterialText.text = rendererUI.material.name.Replace(" (Instance)", "");
    }

    void ApplyChanges(GameObject pipeToModify)
    {
        pipeToModifySettings = pipeToModify.GetComponent<PipeBase>().PipeSettings;
        float radiusToSet = settingsDiameter.value / 2 / 1000;
        pipeToModifySettings.Radius = radiusToSet;
        PipesScript pipeToModifyScript = pipeToModify.GetComponent<PipesScript>();
        pipeToModifyScript.radius = radiusToSet;
        pipeToModifySettings.FlangeDetail.Flange = settingsFlangesSet.isOn;
        pipeToModifySettings.FlangeDetail.Size = radiusToSet / 3;
        pipeToModifySettings.FlangeDetail.Length = radiusToSet * 2;
        pipeToModifySettings.FlangeDetail.Interval = settingsFlangeLength.value;
        pipeToModifySettings.CornersDetail.Size = radiusToSet;
        pipeToModifySettings.CornersDetail.Steps = 6;
        pipeToModify.GetComponent<PipeBase>().Material = settingsMaterial;

        pipeToModifyScript.UpdatePipe();

        pipeToModify.GetComponent<PipesScript>().CalculateCost();
        settingsCostText.text = pipeToModify.GetComponent<PipesScript>().totalCost.ToString("F2");

        pipeToModify.GetComponent<PipesScript>().CalculateTotalLength();
        settingsLengthText.text = pipeToModify.GetComponent<PipesScript>().totalLength.ToString("F2");
    }

    public void Return()
    {
        applyButton.onClick.RemoveAllListeners();
        pipesPanelScreen.SetActive(true);
        pipeSettings.SetActive(false);
        returnButton.SetActive(false);
        appSettingsPanel.SetActive(false);

        UpdateTable();
    }

    void EditPipeScreen(GameObject pipe, GameObject element)
    {
        if (globalVariables.pipeBeingEdited != pipe)
        {
            globalVariables.pipeBeingEdited = pipe;
            globalVariables.ToggleEditPipe();
            gameObject.SetActive(false);
        }
        else
        {
            globalVariables.alert.Alert("This pipe is already on edit mode", 2f, MessageType.Alert);
            Debug.Log("Pipe is en edit mode");
        }
    }
    void DeletePipe(GameObject pipe)
    {
        string pipeName = pipe.name;

        // Show the warning before deleting the pipe
        // Show the warning before deleting the pipe
        warningDialog.ShowWarning(
            "Are you sure you want to delete this pipe?",
            () =>
            {
                // Confirm deletion action
                Destroy(pipe);
                Destroy(pipesPanel.transform.Find(pipeName).gameObject);
            },
            () =>
            {
                // Cancel deletion action
                Debug.Log("Deletion canceled for pipe: " + pipeName);
            }
        );

    }

    public void doExitGame()
    {
        warningDialog.ShowWarning(
            "Are you sure you want to quit?, the progress will be lost",
            () =>
            {   
                Debug.Log("App quit");
                Application.Quit();
            },
            () =>
            {
                // Cancel deletion action
                Debug.Log("Cancel quit");
            }
        );

    }

    // Update is called once per frame
    void Update()
    {

    }
}
