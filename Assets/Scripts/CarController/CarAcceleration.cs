using UnityEngine;

public class CarAcceleration : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 10f;
    public float brakeForce = 20f;

    private float currentSpeed = 0f;

    private void Update()
    {
        // Get input for acceleration, deceleration, and brake
        float accelerateInput = Input.GetAxis("Vertical");
        float decelerateInput = -Input.GetAxis("Vertical");
        bool brakeInput = Input.GetKey(KeyCode.Space);

        // Apply brake if brake input is detected
        if (brakeInput)
        {
            ApplyBrake(brakeForce * Time.deltaTime);
        }
        else
        {
            // Calculate acceleration
            if (accelerateInput > 0f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            }
            // Calculate deceleration
            else if (decelerateInput > 0f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            }
        }

        // Move the car based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    public void ApplyBrake(float force)
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, force);
    }
}
