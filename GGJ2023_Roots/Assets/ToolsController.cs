using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public Camera ToolsCamera;
    public Vector2 ScaleBounds = new Vector2(0.2f, 1);
    public Vector3 RegularScale = new Vector3(1, 1, 1);
    public float DistanceFactor = 0.3f;
    public float SmoothFollowFactor = 7;
    public float SmoothScaleFactor = 7;

    public Tool[] AvailableTools;
    public Tool SelectedTool;
    public int SelectedIndex = 0;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (ToolsCamera == null)
            Debug.LogError("MISSING TOOLS CAMERA REFERENCE");

        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SetTool(SelectedIndex));

        foreach(var tool in AvailableTools)
        {
            tool.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            float scale = Mathf.Clamp( 1/(hit.distance*DistanceFactor), 0.3f,1);
            SelectedTool.Instance.transform.localScale = Vector3.Lerp(SelectedTool.Instance.transform.localScale, new Vector3(scale, scale, scale), SmoothScaleFactor * Time.deltaTime);
        }
        else
        {
            SelectedTool.Instance.transform.localScale = Vector3.Lerp(SelectedTool.Instance.transform.localScale, RegularScale, SmoothScaleFactor * Time.deltaTime);
        }

        var movePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        SelectedTool.Instance.transform.position = Vector3.Lerp(SelectedTool.Instance.transform.position, movePos, SmoothFollowFactor* Time.deltaTime);


        if(Input.GetMouseButtonDown(0))
        {
            SelectedTool.StartUseTool();
            audioSource.clip = SelectedTool.GetRandomAudio();
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SelectedTool.StopUseTool();
            audioSource.Stop();

        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A)) {
            SelectedIndex = (SelectedIndex + 1) % AvailableTools.Length;
            StartCoroutine(SetTool(SelectedIndex));
        }
#endif
    }
    private IEnumerator SetTool(int index)
    {
        if (SelectedTool != null)
        {
            for (float i = 1; i >= 0; i -= 3* Time.deltaTime)
            {
                // set color with i as alpha
                SelectedTool.Instance.transform.localScale = new Vector3(i, i, i);
                yield return null;
            }
            SelectedTool.Instance.SetActive(false);
        }

        SelectedTool = AvailableTools[index];
        SelectedTool.Instance.transform.SetParent(transform);

        SelectedTool.Instance.SetActive(true);
        for (float i = 0; i <= 1; i += 3 * Time.deltaTime)
        {
            SelectedTool.Instance.transform.localScale = new Vector3(i, i, i);
            yield return null;
        }
    }
}
