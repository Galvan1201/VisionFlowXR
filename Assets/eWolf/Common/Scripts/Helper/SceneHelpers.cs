using System;
using UnityEngine;

namespace eWolf.Common.Helper
{
    public class SceneHelpers : MonoBehaviour
    {
        public static void UpdateSnapshot()
        {
            if (Input.GetKeyDown("c"))
            {
                string screenshotFilename;
                DateTime td = DateTime.Now;
                screenshotFilename = "..//ScreenShots//SS - " + td.ToString("yyyy MM dd-HH-mm-ss-ffff") + ".png";
                ScreenCapture.CaptureScreenshot(screenshotFilename);
                Debug.Log("Taken Snap Shot." + td.ToString("yyyy MM dd-HH-mm-ss-ffff"));
            }
        }

        public void Update()
        {
            UpdateSnapshot();
        }
    }
}