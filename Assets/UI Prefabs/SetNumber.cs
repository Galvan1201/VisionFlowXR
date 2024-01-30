using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetNumber : MonoBehaviour
{
    public Button applyButton;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            // Debug.Log(child.transform.Find("Number").Find("Value").GetComponent<TextMeshProUGUI>());
            // Debug.Log(child.transform.Find("Up arrow").GetComponent<Button>());
            // Debug.Log(child.transform.Find("Down arrow").GetComponent<Button>());
            TextMeshProUGUI valueText = child.transform.Find("Number").Find("Value").GetComponent<TextMeshProUGUI>();
            child.transform.Find("Up arrow").GetComponent<Button>().onClick.AddListener(() => Up(valueText));
            child.transform.Find("Down arrow").GetComponent<Button>().onClick.AddListener(() => Down(valueText));
        }
    }

    void Up(TextMeshProUGUI value)
    {
        reactivateButton();
        int intValue;

        if (int.TryParse(value.text, out intValue) && intValue != 9)
        {
            intValue += 1;
            value.text = intValue.ToString();
        }
        else
        {
            value.text = "0";
        }
    }

    void Down(TextMeshProUGUI value)
    {
        reactivateButton();
        int intValue;

        if (int.TryParse(value.text, out intValue) && intValue != 0)
        {
            intValue -= 1;
            value.text = intValue.ToString();
        }
        else
        {
            value.text = "9";
        }
    }

    void reactivateButton()
    {
        if (applyButton && !applyButton.interactable)
        {
            applyButton.interactable = true;
        }
    }

    public float UpdateFinalNumber()
    {
        float total = 0;
        foreach (Transform child in transform)
        {
            string multiplier = child.name;
            string value = child.Find("Number").Find("Value").GetComponent<TextMeshProUGUI>().text;
            int intValue;
            int intMultiplier;
            if (int.TryParse(value, out intValue) && int.TryParse(multiplier, out intMultiplier))
            {
                total += intValue * intMultiplier;
            }
        }
        return total;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
