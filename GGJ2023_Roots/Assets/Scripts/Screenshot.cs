using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            ScreenCapture.CaptureScreenshot($"C:\\Users\\josem\\Pictures\\Saved Pictures\\screenshot_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.png", 4);
        }
    }
}
