using UnityEngine;

public class CarBrake : MonoBehaviour
{
    public float brakeForce = 0.5f;

    private CarController carController;

    private void Start()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            carController.ApplyBrake(brakeForce);
        }
    }
}
