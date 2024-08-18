using UnityEngine;

public class RampAirControl : MonoBehaviour
{
    public float midAirControl = 1f;
    public float rampControl = 0.5f;
    public LayerMask groundLayer;

    private CarController carController;
    private bool isGrounded;

    private void Start()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        float moveInput = Input.GetAxis("Vertical");

        // Check if the car is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);

        // Apply mid-air control if the car is not grounded
        if (!isGrounded)
        {
            transform.position += transform.forward * moveInput * carController.speed * midAirControl * Time.deltaTime;
        }
        // Apply ramp control if the car is grounded and colliding with a ramp
        else if (isGrounded && moveInput != 0 && Mathf.Abs(transform.rotation.eulerAngles.y) < 0.5f)
        {
            transform.position += transform.forward * moveInput * carController.speed * rampControl * Time.deltaTime;
        }
        // Apply normal car control if the car is grounded on regular terrain
        else
        {
            transform.position += transform.forward * moveInput * carController.speed * Time.deltaTime;
        }
    }
}

