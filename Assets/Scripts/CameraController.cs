using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private _CameraSettings cameraSettings;

    [SerializeField] private float heightOffset = 2.5f;
    [SerializeField] private float distanceOffset = 4f;
    [SerializeField] private float cameraTilt = 15f;

    [SerializeField] private float positionSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 5f;

    [Header("Look Ahead")]
    [SerializeField] private float forwardLookAhead = 0.5f;
    [SerializeField] private float turnLookAhead = 2f;

    private Vector2 _inputAxis;
    private bool _isFlipCamera;

    private void Start()
    {
        if (cameraSettings != null)
            UpdateCameraSettings(cameraSettings);
    }

    private void Update()
    {
        _inputAxis.x = Input.GetAxis("Horizontal");
        _inputAxis.y = Input.GetAxis("Vertical");

        // Get "Fire1"
        _isFlipCamera = Input.GetButton("Fire1");
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Set the Position
        Vector3 vertical_offset = target.up * heightOffset;
        //Vector3 backward_offset = -target.forward * distanceOffset;
        Vector3 flip_forward = _isFlipCamera ? target.forward : -target.forward;

        Vector3 backward_offset = distanceOffset * flip_forward;
        Vector3 forward_input_offset = flip_forward * _inputAxis.y * forwardLookAhead;

        Vector3 target_camera_position = target.position + vertical_offset + backward_offset + forward_input_offset;
        transform.position = Vector3.Lerp(transform.position, target_camera_position, positionSmoothing * Time.deltaTime);

        // Set the Rotation
        // Get the Horizontal Input
        Vector3 look_direction = target.position - transform.position;
        Vector3 forward_offset_rotation = -flip_forward * 10f;
        Vector3 turn_input_offset_rotation = _inputAxis.x * turnLookAhead * transform.right;

        Quaternion target_rotation = Quaternion.LookRotation(look_direction + forward_offset_rotation + turn_input_offset_rotation);
        target_rotation *= Quaternion.Euler(cameraTilt, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, rotationSmoothing * Time.deltaTime);
    }

    private void UpdateCameraSettings(_CameraSettings settings)
    {
        cameraSettings = settings;

        heightOffset = settings.heightOffset;
        distanceOffset = settings.distanceOffset;
        cameraTilt = settings.cameraTilt;
        positionSmoothing = settings.positionSmoothing;
        rotationSmoothing = settings.rotationSmoothing;
        forwardLookAhead = settings.forwardLookAhead;
        turnLookAhead = settings.turnLookAhead;
    }

    public void SetTarget(Transform newTarget, _CameraSettings settings)
    {
        target = newTarget;
        UpdateCameraSettings(settings);
    }
}
