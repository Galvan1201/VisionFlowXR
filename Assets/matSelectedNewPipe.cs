using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class matSelectedNewPipe : MonoBehaviour
{
    NewPipeLogic newPipeLogic;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(FindObjectOfType<NewPipeLogic>());
        newPipeLogic = FindObjectOfType<NewPipeLogic>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(click);
    }

    // Update is called once per frame
    void click()
    {   
        newPipeLogic.ChangeMaterial(gameObject.GetComponent<Renderer>());
    }
}