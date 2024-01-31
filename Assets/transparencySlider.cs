using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class transparencySlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
        Slider slider = gameObject.transform.parent.GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(value => text.text = (value*100).ToString("F0"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
