using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera rearCamera;
    [SerializeField] private AudioListener rearAudioListener;
    [SerializeField] private _CameraSettings cameraSettings;

    [SerializeField] private float heightOffset = 2.5f;
    [SerializeField] private float distanceOffset = 4f;
    [SerializeField] private float cameraTilt = 15f;
    [SerializeField] float _rearDistanceOffset = 2.5f;
    [SerializeField] float _rearCameraHeightOffset = 2;

    [SerializeField] private float positionSmoothing = 10f;
    [SerializeField] private float rotationSmoothing = 5f;

    [Header("Look Ahead")]
    [SerializeField] private float forwardLookAhead = 0.5f;
    [SerializeField] private float turnLookAhead = 2f;

    private Vector2 _inputAxis;
    private bool _isFlipCamera;
    private bool _lastFrameFlipCamera;

    private Camera _thisCamera;
    private AudioListener _thisAudioListener;

    private void Start()
    {
        _thisCamera = GetComponent<Camera>();
        _thisAudioListener = GetComponent<AudioListener>();

        if (cameraSettings != null)
            UpdateCameraSettings(cameraSettings);
    }

    private void Update()
    {
        _inputAxis.x = Input.GetAxis("Horizontal");
        _inputAxis.y = Input.GetAxis("Vertical");

        // Get "Fire1"

        bool t = Input.mousePresent;
        if ((Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.LeftControl)))
            _isFlipCamera = true;
        else
            _isFlipCamera = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        SetCameraValues(_thisCamera, false);
        SetCameraValues(rearCamera, true);

        if (_isFlipCamera)
        {
            rearCamera.enabled = true;
            _thisCamera.enabled = false;

            rearAudioListener.enabled = true;
            _thisAudioListener.enabled = false;
        }
        else
        {
            rearCamera.enabled = false;
            _thisCamera.enabled = true;

            rearAudioListener.enabled = false;
            _thisAudioListener.enabled = true;
        }

        //// Set the Position
        //Vector3 vertical_offset = target.up * heightOffset;
        ////Vector3 backward_offset = -target.forward * distanceOffset;

        //Vector3 flip_forward = _isFlipCamera ? target.forward : -target.forward;

        //Vector3 backward_offset = distanceOffset * flip_forward;
        //Vector3 forward_input_offset = flip_forward * _inputAxis.y * forwardLookAhead;

        //Vector3 target_camera_position = target.position + vertical_offset + backward_offset + forward_input_offset;
        //transform.position = Vector3.Lerp(transform.position, target_camera_position, positionSmoothing * Time.deltaTime);

        //// Set the Rotation
        //// Get the Horizontal Input
        //Vector3 look_direction = target.position - transform.position;
        //Vector3 forward_offset_rotation = -flip_forward * 10f;
        //Vector3 turn_input_offset_rotation = _inputAxis.x * turnLookAhead * transform.right;

        ////bool instant_flip = _isFlipCamera != _lastFrameFlipCamera;

        //Quaternion target_rotation = Quaternion.LookRotation(look_direction + forward_offset_rotation + turn_input_offset_rotation);
        //target_rotation *= Quaternion.Euler(cameraTilt, 0, 0);

        ////if (instant_flip)
        ////{
        ////    transform.rotation = target_rotation;
        ////}
        ////else
        ////{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, rotationSmoothing * Time.deltaTime);
        ////}

        ////_lastFrameFlipCamera = _isFlipCamera;
    }

    private void SetCameraValues(Camera camera, bool isRearCamera)
    {
        // Set the Position
        Vector3 vertical_offset = target.up * (isRearCamera? _rearCameraHeightOffset : heightOffset);

        Vector3 flip_forward = isRearCamera ? target.forward : -target.forward;

        Vector3 backward_offset = isRearCamera? _rearDistanceOffset * flip_forward: distanceOffset * flip_forward;

        Vector3 forward_input_offset = flip_forward * _inputAxis.y * forwardLookAhead;

        Vector3 target_camera_position = target.position + vertical_offset + backward_offset + forward_input_offset;
        camera.transform.position = Vector3.Lerp(camera.transform.position, target_camera_position, positionSmoothing * Time.deltaTime);

        // Set the Rotation
        // Get the Horizontal Input
        Vector3 look_direction = target.position - camera.transform.position;
        Vector3 forward_offset_rotation = -flip_forward * 10f;
        Vector3 turn_input_offset_rotation = _inputAxis.x * turnLookAhead * camera.transform.right;

        Quaternion target_rotation = Quaternion.LookRotation(look_direction + forward_offset_rotation + turn_input_offset_rotation);
        target_rotation *= Quaternion.Euler(cameraTilt, 0, 0);

        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, target_rotation, rotationSmoothing * Time.deltaTime);
    }

    private void UpdateCameraSettings(_CameraSettings settings)
    {
        cameraSettings = settings;

        heightOffset = settings.heightOffset;
        distanceOffset = settings.distanceOffset;
        _rearDistanceOffset=settings.rearDistanceOffset;
        _rearCameraHeightOffset = settings.rearCameraHeightOffset;
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
