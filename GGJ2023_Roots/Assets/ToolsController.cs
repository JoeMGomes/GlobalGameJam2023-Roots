using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public Camera ToolsCamera;
    public GameObject SelectedTool;
    public Vector2 ScaleBounds = new Vector2(0.2f, 1);
    public Vector3 RegularScale = new Vector3(1, 1, 1);
    public float DistanceFactor = 0.3f;
    public float SmoothFollowFactor = 7;
    public float SmoothScaleFactor = 7;

    // Start is called before the first frame update
    void Start()
    {
        if (ToolsCamera == null)
            Debug.LogError("MISSING TOOLS CAMERA REFERENCE");

        if (SelectedTool == null)
            Debug.LogError("MISSING SELECTED TOOL REFERENCE");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            float scale = Mathf.Clamp( 1/(hit.distance*DistanceFactor), 0.3f,1);
            SelectedTool.transform.localScale = Vector3.Lerp(SelectedTool.transform.localScale, new Vector3(scale, scale, scale), SmoothScaleFactor * Time.deltaTime);
        }
        else
        { 
            SelectedTool.transform.localScale = Vector3.Lerp(SelectedTool.transform.localScale, RegularScale, SmoothScaleFactor * Time.deltaTime);
        }

        var movePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        SelectedTool.transform.position = Vector3.Lerp(SelectedTool.transform.position, movePos, SmoothFollowFactor* Time.deltaTime);
        

    }
}
