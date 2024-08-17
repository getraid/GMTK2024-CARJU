using UnityEngine;

public class DownForceSpeed : MonoBehaviour
{
    public float downForce = 10f;
    private Rigidbody carRigidbody;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Apply downforce based on car's speed
        float speed = carRigidbody.velocity.magnitude;
        carRigidbody.AddForce(Vector3.down * downForce * speed);
    }
}
