using System;
using UnityEngine;
using UnityEngine.UI;

// Display FPS on a Unity UGUI Text Panel
// To use: Drag onto a game object with Text component
//         Press 'F' key to toggle show/hide
public class FPSCounterV1 : MonoBehaviour
{
    const int targetFPS = 90;

    const float updateInterval = 0.5f;

    int framesCount;
    float framesTime;
    bool show = true;
    Text text;

    void Start()
    {
        // no text object set? see if our game object has one to use
        if (text == null) text = GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) show = !show;

        // monitoring frame counter and the total time
        framesCount++;
        framesTime += Time.unscaledDeltaTime;

        // measuring interval ended, so calculate FPS and display on Text
        if (framesTime > updateInterval)
        {
            if (text != null)
            {
                if (show)
                {
                    var fps = framesCount / framesTime;

                    UpdateFPSText(fps);
                    ColorText(fps);
                }
                else
                {
                    text.text = string.Empty;
                }
            }

            // reset for the next interval to measure
            framesCount = 0;
            framesTime = 0;
        }
    }

    void UpdateFPSText(float fps)
    {
        text.text = string.Format("{0:F1} FPS", fps);
    }

    void ColorText(float fps)
    {
        text.color = fps > targetFPS - 30 ? Color.green :
            fps > targetFPS - 60 ? Color.yellow :
            Color.red;
    }
}