using UnityEngine;

public class CarRollOver : MonoBehaviour
{
    public float rollOverThreshold = 80f;
    public float flipTorque = 1000f;
    public LayerMask groundLayer;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check if the car has rolled over
        if (IsCarRolledOver())
        {
            // Apply a torque to flip the car back over
            rb.AddTorque(transform.forward * flipTorque);
        }
    }

    private bool IsCarRolledOver()
    {
        // Check the car's up vector against the world up vector
        float dotProduct = Vector3.Dot(transform.up, Vector3.up);

        // If the dot product is below the threshold, the car is considered rolled over
        return dotProduct < Mathf.Cos(rollOverThreshold * Mathf.Deg2Rad);
    }
}
