using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Turntable: MonoBehaviour
{
    //Rotation
    public float RotateSensitivity = 10; 
    public float ZoomSensitivity = 10; 
    public float DampForce = 20;
    private Vector3 MouseClickPoint;
    private bool IsRotating;
    public float RotationSpeed;

    //Zoom
    public CinemachineVirtualCamera cam;
    public CinemachineTrackedDolly CamDolly;
    public float ZoomSpeed = 3f;

    void Start()
    {
        RotationSpeed = 0;
        if(cam == null)
        {
            Debug.LogError("NO VIRTUAL CAMERA SELECT FOR TURNTABLE");
        }

        CamDolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();

        if(CamDolly == null)
        {
            Debug.LogError("VIRTUAL CAMERA HAS NO DOLLY TRACK");
        }
    }

    void Update()
    {
        if (IsRotating)
        {
            var mouseOffset = (Input.mousePosition - MouseClickPoint);

            RotationSpeed = -(mouseOffset.x + mouseOffset.y) * RotateSensitivity;

            MouseClickPoint = Input.mousePosition;
        }


        if(RotationSpeed > 0)
            RotationSpeed -= DampForce * Time.deltaTime;
        else
            RotationSpeed += DampForce * Time.deltaTime;

        if (Mathf.Abs(RotationSpeed) < 0.3)
            RotationSpeed = 0;

        transform.Rotate( 0, RotationSpeed * Time.deltaTime,0 );

        CamDolly.m_PathPosition += Input.mouseScrollDelta.y * ZoomSensitivity * Time.deltaTime;
        CamDolly.m_PathPosition = Mathf.Clamp(CamDolly.m_PathPosition, 0, 1);
    }

    void OnMouseDown()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100) && hit.collider.CompareTag("world"))
        {
            IsRotating = true;
            MouseClickPoint = Input.mousePosition;
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
        }

    }

    void OnMouseUp()
    {
        IsRotating = false;
    }

}
