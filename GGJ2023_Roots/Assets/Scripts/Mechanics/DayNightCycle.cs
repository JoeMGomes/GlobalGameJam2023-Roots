using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sunLight;
    public Material skyboxMaterial;
    private Material originalMaterial;
    private Material currentMaterial;
    public Transform sunMoonPivot;
    public CanvasGroup fastForwardIcon;
    public float speed;

    [Header("Light Settings")]
    public Color daylightColor;
    public Color nightLightColor;
    
    private Transform sunLightTransform;
    [Header("Skybox Settings")]
    public Color dayTopColor;
    public Color dayMiddleColor;
    public Color dayBottomColor;
    public Color nightTopColor;
    public Color nightMiddleColor;
    public Color nightBottomColor;

    int skyboxTopColorShaderID = Shader.PropertyToID("_TopColor");
    int skyboxMiddleColorShaderID = Shader.PropertyToID("_MiddleColor");
    int skyboxBottomColorShaderID = Shader.PropertyToID("_BottomColor");

    public void Awake()
    {
        sunLightTransform = sunLight.transform;
        currentMaterial = new Material(skyboxMaterial.shader);
        originalMaterial = RenderSettings.skybox;
        RenderSettings.skybox = currentMaterial;
        RenderSettings.skybox.CopyPropertiesFromMaterial(skyboxMaterial);
    }


    public void Update()
    {
        Vector3 sunLightRotation = sunLightTransform.localRotation.eulerAngles;
        sunLightRotation.x += speed * Time.deltaTime;
        sunLightTransform.Rotate(Vector3.right, speed * Time.deltaTime);
        sunMoonPivot.Rotate(Vector3.forward, speed * Time.deltaTime);
        sunLight.color = Color.Lerp(daylightColor, nightLightColor, sunLightRotation.x / 180);
        float duration = 180 / speed;
        float lerpDelta = Mathf.PingPong(Time.time, duration) / duration;
        RenderSettings.skybox.SetColor(skyboxTopColorShaderID, Color.Lerp(dayTopColor, nightTopColor, lerpDelta));
        RenderSettings.skybox.SetColor(skyboxMiddleColorShaderID, Color.Lerp(dayMiddleColor, nightMiddleColor, lerpDelta));
        RenderSettings.skybox.SetColor(skyboxBottomColorShaderID, Color.Lerp(dayBottomColor, nightBottomColor, lerpDelta));

        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = 3;
            fastForwardIcon.alpha = 1;
        }
        else
        {
            Time.timeScale = 1;
            fastForwardIcon.alpha = 0;
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void OnDestroy()
    {
        RenderSettings.skybox = originalMaterial;
    }
}
