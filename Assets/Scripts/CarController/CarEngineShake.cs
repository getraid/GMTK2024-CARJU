using UnityEngine;

public class CarEngineShake : MonoBehaviour
{
    public Transform carBody;
    public float shakeSpeed = 10f;
    public float shakeAmount = 0.1f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = carBody.localPosition;
    }

    private void Update()
    {
        // Calculate the shaking displacement
        float displacement = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
        Vector3 shakeDisplacement = new Vector3(displacement, 0f, displacement) * shakeAmount;

        // Apply the shaking displacement to the car's position
        carBody.localPosition = originalPosition + shakeDisplacement;
    }
}
