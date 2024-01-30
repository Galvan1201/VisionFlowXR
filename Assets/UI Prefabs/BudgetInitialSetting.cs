using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BudgetInitialSetting : MonoBehaviour
{
    CanvasGroup canvasGroup;
    private System.Action onConfirmAction;
    private System.Action onCancelAction;
    SetNumber setNumber;
    public GlobalVariables globalVariables;
    TextMeshProUGUI currentTotalText;
    TextMeshProUGUI remainingText;
    Canvas canvas;

    public Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.transform.Find("Continue").GetComponent<Button>().onClick.AddListener(Continue);
        setNumber = gameObject.transform.GetComponentInChildren<SetNumber>();
        currentTotalText = gameObject.transform.Find("totalValue").GetComponent<TextMeshProUGUI>();
        remainingText = gameObject.transform.Find("remainingValue").GetComponent<TextMeshProUGUI>();
        gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(Close);
    }

    public void ShowBudgetScreen()
    {
        // Ensure the canvas group is active
        canvas.enabled = true;
        canvasGroup.alpha = 0f;

        // Set the alert canvas in front of the camera
        Transform cameraTransform = camera.transform;
        transform.position = cameraTransform.position + cameraTransform.forward * 0.7f; // Adjust the distance as needed
        UpdateNumbers();
        // Start the fade-in animation
        StartCoroutine(FadeIn());

        // Start the fade-out animation after the specified duration
        
    }

    void UpdateNumbers()
    {
        globalVariables.budgetUpdate();
        currentTotalText.text = globalVariables.currentCost.ToString("F2");
        remainingText.text = globalVariables.remainingBudget.ToString("F2");
        if (globalVariables.remainingBudget < 0)
        {
            remainingText.color = Color.red;
        }
        else
        {
            remainingText.color = Color.white;
        }
    }
    
    public void Continue()
    {
        globalVariables.budget = setNumber.UpdateFinalNumber();
        globalVariables.budgetUpdate();
        gameObject.transform.Find("Continue").GetComponent<Button>().interactable = false;
        UpdateNumbers();
    }

    IEnumerator FadeIn()
    {
        // Use a loop to gradually increase the alpha value
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime; // Adjust the speed of the fade

            yield return null;
        }

        // Ensure the alpha is set to 1
        canvasGroup.alpha = 1f;
    }

    void Close()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Use a loop to gradually decrease the alpha value
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime; // Adjust the speed of the fade

            yield return null;
        }

        // Ensure the alpha is set to zero
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
    }
}
