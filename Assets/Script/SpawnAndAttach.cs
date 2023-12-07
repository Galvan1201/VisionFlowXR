using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
 
 
public class Spawn : MonoBehaviour
{
    public InputActionReference toggleReference = null;
    private bool toggleBigTriggered = false;
    public GameObject item;
    public Transform Player;
    public Transform RightHand;
 
 
    private void Awake()
    {
        toggleReference.action.Enable();
    }
 
 
    void Start()
    {
        // Add toggleReference.action.started to the Start() method
        toggleReference.action.started += ToggleBigAction;
    }
 
    public void ToggleBigAction(InputAction.CallbackContext context)
    {
        // Check if toggleBigTriggered is false
        if (!toggleBigTriggered)
        {
            // Instantiate the item at the right hand's position
            Instantiate(item, RightHand.position, Quaternion.identity);
 
            // Set toggleBigTriggered to true
            toggleBigTriggered = true;
        }
 
        UnityEngine.Debug.Log("Big action toggled!");
    }
 
}