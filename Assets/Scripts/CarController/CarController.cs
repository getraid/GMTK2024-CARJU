using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float boostSpeed = 20f;
    public float rotationSpeed = 100f;
    public float brakeForce = 30f; // Increase brake force

    private bool isBoosting = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool boostInput = Input.GetKey(KeyCode.LeftShift);
        bool brakeInput = Input.GetKey(KeyCode.Space);

        // Determine the current speed based on boost input
        float currentSpeed = isBoosting ? boostSpeed : speed;

        // Calculate the forward movement
        float moveAmount = currentSpeed * moveInput * Time.deltaTime;
        rb.MovePosition(transform.position + transform.forward * moveAmount);
        

        // Calculate the rotation
        float turnAmount = rotationSpeed * turnInput * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Check for boost input
        if (boostInput && !isBoosting)
        {
            isBoosting = true;
            speed = boostSpeed; // Set the speed to boost speed
        }
        else if (!boostInput && isBoosting)
        {
            isBoosting = false;
            speed = speed; // Reset the speed back to default
        }

        // Apply brake if brake input is detected
        if (brakeInput)
        {
            ApplyBrake(brakeForce * Time.deltaTime); // Apply brake force over time
        }
    }

    public void ApplyBrake(float force)
    {
        rb.velocity -= rb.velocity * force; // Decelerate the car gradually
    }
}
