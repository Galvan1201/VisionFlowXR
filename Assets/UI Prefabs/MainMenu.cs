using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using eWolf.PipeBuilder.VisionFlowScripts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject pipesPanel;
    public GlobalVariables globalVariables;
    [SerializeField] private GameObject pipeElementInList;

    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateTable();
    }

    void UpdateTable()
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
            generatedElement.transform.Find("Configure").GetComponent<Button>().onClick.AddListener(PipeSettings);
            generatedElement.transform.Find("Edit").GetComponent<Button>().onClick.AddListener(() => EditPipeScreen(pipe, generatedElement));
            generatedElement.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(() => DeletePipe(pipe));
            if(globalVariables.pipeBeingEdited == pipe)
            {
                generatedElement.GetComponent<Image>().color = new Color(132f / 255f, 231f / 255f, 213f / 255f, 1f); // This is the color #F7B142;
            }
        }
        gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(Close);
    }

    void OnDisable()
    {

    }

    void Close()
    {
        gameObject.SetActive(false);
    }

    void PipeSettings()
    {
        Debug.Log("PipeSettings");
    }
    void EditPipeScreen(GameObject pipe, GameObject element)
    {
        if(globalVariables.pipeBeingEdited != pipe)
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
        Destroy(pipe);
        Destroy(pipesPanel.transform.Find(pipeName).gameObject);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
