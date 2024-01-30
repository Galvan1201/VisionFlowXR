using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType
{
    Message,
    Alert,
    Error
}

public class AlertHandler : MonoBehaviour
{
    TextMeshProUGUI textComp;
    CanvasGroup canvasGroup;
    Image backgroundImage;
    public Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        textComp = transform.Find("Background").GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        backgroundImage = transform.Find("Background").GetComponent<Image>();
    }

    public void Alert(string message, float duration, MessageType messageType)
    {
        textComp.text = message;

        // Set background color based on message type
        SetBackgroundColor(messageType);

        // Ensure the canvas group is active
        canvasGroup.alpha = 0f;

        // Set the alert canvas in front of the camera
        Transform cameraTransform = camera.transform;
        transform.position = cameraTransform.position + cameraTransform.forward * 0.1f; // Adjust the distance as needed

        // Start the fade-in animation
        StartCoroutine(FadeIn());

        // Start the fade-out animation after the specified duration
        StartCoroutine(FadeOutAfterDelay(duration));
    }

    void SetBackgroundColor(MessageType messageType)
    {
        Color backgroundColor;

        switch (messageType)
        {
            case MessageType.Message:
                backgroundColor = Color.green;
                break;
            case MessageType.Alert:
                backgroundColor = Color.yellow;
                break;
            case MessageType.Error:
                backgroundColor = Color.red;
                break;
            default:
                backgroundColor = Color.white; // Default color
                break;
        }

        // Set the background color
        backgroundImage.color = backgroundColor;
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

    IEnumerator FadeOutAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Start the fade-out animation
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
    }
}
