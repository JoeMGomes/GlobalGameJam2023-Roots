using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Turntable: MonoBehaviour
{
    private Vector3 MouseClickPoint;
    public float YOfsset;
    //Rotation
    public float RotateSensitivity = 10; 
    public float RotationDampForce = 20;
    private bool IsRotating;
    public float RotationSpeed;
    public float MaxRotationSpeed = 180;

    //Panning
    public Transform LookTarget;
    public float PanningSensitivity = 0.4f;
    public float PanningDampForce = 1.5f;
    private bool IsPanning;
    private Vector2 PanningSpeed;
    public Vector2 panningBounds = new Vector2(-3, 3);


    //Zoom
    public CinemachineVirtualCamera cam;
    private CinemachineTrackedDolly CamDolly;
    public float ZoomSpeed = 3f;
    public float ZoomSensitivity = 10;

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

        if(LookTarget == null)
            Debug.LogError("LOOK TARGET IS NULL");

        YOfsset = LookTarget.position.y;

    }

    void Update()
    {
        bool startRot = Input.GetMouseButtonDown(0);
        bool startPan = Input.GetMouseButtonDown(1);

        if (startRot  || startPan)
        {
            IsRotating = startRot;
            IsPanning = startPan; 
            MouseClickPoint = Input.mousePosition;            
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsRotating = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            IsPanning = false;
        }

        if (IsRotating || IsPanning)
        {
            var mouseOffset = (Input.mousePosition - MouseClickPoint);
            RotationSpeed = Math.Clamp(IsRotating ? -(mouseOffset.x + mouseOffset.y) * RotateSensitivity : RotationSpeed, -MaxRotationSpeed, MaxRotationSpeed);
            PanningSpeed = IsPanning ? -((Vector2)mouseOffset) * PanningSensitivity : PanningSpeed;
            MouseClickPoint = Input.mousePosition;
        }

        if(RotationSpeed > 0)
            RotationSpeed -= RotationDampForce * Time.deltaTime;
        else
            RotationSpeed += RotationDampForce * Time.deltaTime;

        if (Mathf.Abs(RotationSpeed) < 0.3)
            RotationSpeed = 0;

        if (PanningSpeed.x > 0)
        {
            PanningSpeed.x -= PanningDampForce * Time.deltaTime;
        }
        else
        {
            PanningSpeed.x += PanningDampForce * Time.deltaTime;
        }

        if (PanningSpeed.y > 0)
        {
            PanningSpeed.y -= PanningDampForce * Time.deltaTime;
        }
        else
        {
            PanningSpeed.y += PanningDampForce * Time.deltaTime;
        }

        if (Mathf.Abs(PanningSpeed.magnitude) < 0.1)
            PanningSpeed = Vector2.zero;


        transform.Rotate( 0, RotationSpeed * Time.deltaTime,0);

        CamDolly.m_PathPosition += Input.mouseScrollDelta.y * ZoomSensitivity * Time.deltaTime;
        CamDolly.m_PathPosition = Mathf.Clamp(CamDolly.m_PathPosition, 0, 1);

        LookTarget.position = new Vector3(Mathf.Clamp(LookTarget.position.x + PanningSpeed.x * PanningSensitivity * Time.deltaTime, panningBounds.x, panningBounds.y),
                                          Mathf.Clamp(LookTarget.position.y + PanningSpeed.y * PanningSensitivity * Time.deltaTime, panningBounds.x + YOfsset, panningBounds.y + YOfsset),
                                          LookTarget.position.z);
    }

}
