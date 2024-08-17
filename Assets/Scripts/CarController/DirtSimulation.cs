using UnityEngine;

public class DirtSimulation : MonoBehaviour
{
    public float dirtFriction = 0.5f;
    public float dirtDrag = 0.2f;
    public ParticleSystem dirtParticles;

    private Rigidbody rb;
    private bool isOnDirt = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dirt"))
        {
            // Enable dirt effects when colliding with a dirt object
            isOnDirt = true;
            dirtParticles.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dirt"))
        {
            // Disable dirt effects when leaving the dirt object
            isOnDirt = false;
            dirtParticles.Stop();
        }
    }

    private void FixedUpdate()
    {
        // Apply dirt-specific physics modifications when on dirt
        if (isOnDirt)
        {
            // Adjust friction
            rb.drag = dirtDrag;
            rb.angularDrag = dirtDrag;
            rb.AddForce(-rb.velocity * dirtFriction, ForceMode.Force);
        }
        else
        {
            // Reset physics parameters when not on dirt
            rb.drag = 0f;
            rb.angularDrag = 0.05f;
        }
    }
}
