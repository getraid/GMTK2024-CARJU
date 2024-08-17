using UnityEngine;

public class Handbrake : MonoBehaviour
{
    public float handbrakeForce = 20f;
    public float fishTailForce = 5f;

    private bool isHandbrakeEngaged = false;

    private void Update()
    {
        // Check for handbrake input
        if (Input.GetKey(KeyCode.E))
        {
            isHandbrakeEngaged = true;
            ApplyHandbrake();
        }
        else
        {
            isHandbrakeEngaged = false;
        }
    }

    private void ApplyHandbrake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Get input for acceleration
        float accelerateInput = Input.GetAxis("Vertical");

        // Slow down the car gradually while acceleration input is active
        if (accelerateInput > 0f)
        {
            rb.velocity *= 1f - handbrakeForce * Time.deltaTime;
        }
        // Stop the car if no acceleration input is detected
        else
        {
            // Check if the car is already stopped
            if (rb.velocity.magnitude > 0.01f)
            {
                rb.velocity = Vector3.zero;
            }
        }

        // Apply fish tail effect if handbrake is engaged
        if (isHandbrakeEngaged)
        {
            float fishTailTorque = fishTailForce * Input.GetAxis("Horizontal");
            rb.AddTorque(transform.up * fishTailTorque);
        }
    }

 }
