using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Turntable: MonoBehaviour
{
    private float Sensitivity = 5; 
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private bool _isRotating;

    public float RotationSpeed;
    public float DampForce = 75;

    public Vector2 FOVBounds = new Vector2(30, 90);
    public float ZoomSpeed = 3f;

    void Start()
    {
        RotationSpeed = 0;
    }

    void Update()
    {
        if (_isRotating)
        {
            _mouseOffset = (Input.mousePosition - _mouseReference);

            RotationSpeed = -(_mouseOffset.x + _mouseOffset.y) * Sensitivity;

            _mouseReference = Input.mousePosition;
        }


        if(RotationSpeed> 0)
            RotationSpeed -= DampForce * Time.deltaTime;
        else
            RotationSpeed += DampForce * Time.deltaTime;

        if (Mathf.Abs(RotationSpeed) < 0.3)
            RotationSpeed = 0;

        transform.Rotate( 0, RotationSpeed * Time.deltaTime,0 );

        Camera.main.fieldOfView += Input.mouseScrollDelta.y * ZoomSpeed * Time.deltaTime;

        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, FOVBounds.x, FOVBounds.y);
    }

    void OnMouseDown()
    {
        _isRotating = true;
        _mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        _isRotating = false;
    }

}
