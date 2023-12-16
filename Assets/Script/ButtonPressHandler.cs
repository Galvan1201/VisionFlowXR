using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Wave.Essence.ScenePerception.Sample;
public class ButtonPressHandler : MonoBehaviour
{
    private InputAction _passthrough;
    public PassThroughHelper passThroughHelper;
    public InputActionAsset inputActions;
    private bool isOn;
    private void OnEnable()
    {
        // Enable the input action
       _passthrough = inputActions.FindActionMap("XRI RightHand").FindAction("Passthrough");
       _passthrough.Enable();
       _passthrough.performed += OnButtonPress;
    }

    private void OnDisable()
    {
        // Disable the input action
        inputActions.FindActionMap("XRI RightHand").FindAction("Passthrough").Disable();
    }


    private void OnButtonPress(InputAction.CallbackContext context)
    {   
        // Handle the button press here
        if(isOn)
        {
        passThroughHelper.HidePassthroughUnderlay();
        }
        if(!isOn)
        {
        passThroughHelper.ShowPassthroughUnderlay(true); 
        }
        isOn = !isOn;
    }
}
