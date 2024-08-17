using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float heightOffset = 1f;
    [SerializeField] private float distanceOffset = 1.5f;
    [SerializeField] private float cameraTilt = 10f;

    [SerializeField] private float positionSmoothing = 3f;
    [SerializeField] private float rotationSmoothing = 2f;

    Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Set the Position - Based on the target's forward direction
        //Vector3 target_position = target.TransformPoint(new Vector3(0f, heightOffset, -distanceOffset));
        //transform.position = Vector3.SmoothDamp(target_position, transform.position, ref _velocity, positionSmoothing * Time.deltaTime);

        Vector3 target_camera_position = target.position + target.up * heightOffset - target.forward * distanceOffset;
        transform.position = Vector3.Lerp(transform.position, target_camera_position, positionSmoothing * Time.deltaTime);

        // Set the Rotation
        Quaternion target_rotation = Quaternion.LookRotation(target.position - transform.position + target.forward * 10f);
        target_rotation *= Quaternion.Euler(cameraTilt, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, rotationSmoothing * Time.deltaTime);
    }
}
