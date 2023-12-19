using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class AlertHandler : MonoBehaviour
{
    TextMeshProUGUI textComp;
    // Start is called before the first frame update
    void Awake()
    {
        textComp = transform.Find("Background").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Alert(string message)
    {   
        gameObject.GetComponent<Canvas>().enabled = true;
        textComp.text = message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
