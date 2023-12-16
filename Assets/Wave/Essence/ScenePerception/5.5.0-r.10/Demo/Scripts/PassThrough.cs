using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Native;

public class PassThrough : MonoBehaviour
{

    public void PassthroughUnderlay(bool active)
    {
        if(active == true)
        {
            Wave.Native.Interop.WVR_ShowPassthroughUnderlay(false); // Show Passthrough Underlay
        }
        else{
            Wave.Native.Interop.WVR_ShowPassthroughUnderlay(true); // Hide Passthrough Underlay
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PassthroughUnderlay(Wave.Native.Interop.WVR_IsPassthroughOverlayVisible());
    }
}
