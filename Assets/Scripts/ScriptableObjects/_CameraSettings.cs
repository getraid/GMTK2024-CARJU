using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Data/CameraSettings")]
public class _CameraSettings : ScriptableObject
{
    public float heightOffset = 2.5f;
    public float distanceOffset = 4f;
    public float rearDistanceOffset = 4f;
    public float rearCameraHeightOffset = 2f;
    public float cameraTilt = 10f;

    public float positionSmoothing = 10f;
    public float rotationSmoothing = 5f;

    public float forwardLookAhead = 0.5f;
    public float turnLookAhead = 2f;
}
