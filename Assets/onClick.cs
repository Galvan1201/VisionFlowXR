using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onClick : MonoBehaviour
{
    // Start is called before the first frame update
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(click);
        Debug.Log(button);
    }

    // Update is called once per frame
    void click()
    {   
        Debug.Log("click");
    }
}
