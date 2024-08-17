using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float rotationSpeed = 2f;
    public float heightDamping = 2f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position based on the target's position and offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * heightDamping);
        transform.position = smoothedPosition;

        // Calculate the desired rotation based on the target's position
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);

        // Smoothly rotate the camera towards the desired rotation
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = smoothedRotation;
    }
}
