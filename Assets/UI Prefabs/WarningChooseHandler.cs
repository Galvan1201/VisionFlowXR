using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarningChooseHandler : MonoBehaviour
{
    public TextMeshProUGUI textComp;
    CanvasGroup canvasGroup;
    Canvas canvas;
    private System.Action onConfirmAction;
    private System.Action onCancelAction;

    public Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }

    public void ShowWarning(string message, System.Action onConfirm, System.Action onCancel)
    {
        textComp.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;
        // Ensure the canvas group is active
        canvasGroup.alpha = 0f;

        // Set the alert canvas in front of the camera
        Transform cameraTransform = camera.transform;
        transform.position = cameraTransform.position + cameraTransform.forward * 0.3f; // Adjust the distance as needed

        // Start the fade-in animation
        StartCoroutine(FadeIn());

        // Start the fade-out animation after the specified duration
        
    }

        public void Confirm()
    {
        onConfirmAction?.Invoke();
        StartCoroutine(FadeOut());
    }

    public void Cancel()
    {
        onCancelAction?.Invoke();
        StartCoroutine(FadeOut());
    }

    

    IEnumerator FadeIn()
    {
        canvas.enabled = true;
        // Use a loop to gradually increase the alpha value
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime; // Adjust the speed of the fade

            yield return null;
        }

        // Ensure the alpha is set to 1
        canvasGroup.alpha = 1f;
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
