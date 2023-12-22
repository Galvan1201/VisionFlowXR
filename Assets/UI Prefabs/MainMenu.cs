using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using eWolf.PipeBuilder.VisionFlowScripts;
using TMPro;
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
            generatedElement.transform.Find("lengthValue").GetComponent<TextMeshProUGUI>().text = "25";
            generatedElement.transform.Find("Configure").GetComponent<Button>().onClick.AddListener(PipeSettings);
            generatedElement.transform.Find("Edit").GetComponent<Button>().onClick.AddListener(() => EditPipeScreen(pipe));
            generatedElement.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(DeletePipe);
        }
    }

    void OnDisable()
    {

    }

    void PipeSettings()
    {
        Debug.Log("PipeSettings");
    }
    void EditPipeScreen(GameObject pipe)
    {
        if(globalVariables.pipeBeingEdited != pipe)
        {
            globalVariables.pipeBeingEdited = pipe;
            globalVariables.ToggleEditPipe();
        }
        else
        {
            Debug.Log("Pipe is en edit mode");
        }
    }
    void DeletePipe()
    {
        Debug.Log("deletePipe");
    }


    // Update is called once per frame
    void Update()
    {

    }
}
