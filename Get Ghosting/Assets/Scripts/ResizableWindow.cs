using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizableWindow : MonoBehaviour
{
    private int lastWidth = 0;
    private int lastHeight = 0;


    void Update ()
    {
        var width = Screen.width;
        var height = Screen.height;

        if(lastWidth != width) // if the user is changing the width
        {
            // update the height
            var heightAccordingToWidth = width / 16.0f * 9.0f;
            Screen.SetResolution(width, (int)Mathf.Round(heightAccordingToWidth), false, 0);
        }
        else if(lastHeight != height) // if the user is changing the height
        {
            // update the width
            var widthAccordingToHeight = height / 9.0f * 16.0f;
            Screen.SetResolution((int)Mathf.Round(widthAccordingToHeight), height, false, 0);
        }

        lastWidth = width;
        lastHeight = height;
    }
}
