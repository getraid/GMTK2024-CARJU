using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] VehicleController controlledVehicle;

    private float _forwardInput;
    private float _reverseInput;
    private float _turnInput;
    private bool _isBraking;

    void Update()
    {
        // Show all Input Keys
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }

        if (controlledVehicle == null)
            return;


        // Accelerating/Reversing
        _forwardInput = Input.GetAxis("Accelerate");
        _reverseInput = Input.GetAxis("Reverse");
        
        if (_forwardInput <= 0f)
        {
            // Check for W key
            if (Input.GetKey(KeyCode.W))
            {
                _forwardInput = 1f;
            }
        }

        if (_reverseInput <= 0f)
        {
            // Check for S key
            if (Input.GetKey(KeyCode.S))
            {
                _reverseInput = 1f;
            }
        }

        // Turning
        _turnInput = Input.GetAxisRaw("Horizontal");

        // Braking
        _isBraking = false;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.Joystick1Button0))
        {
            _isBraking = true;
        }

        // Update Vehicle Input
        ApplyInputToVehicle();
    }

    void ApplyInputToVehicle()
    {
        controlledVehicle.SetForwardInput(_forwardInput);
        controlledVehicle.SetReverseInput(_reverseInput);
        controlledVehicle.SetTurnInput(_turnInput);
        controlledVehicle.SetIsBraking(_isBraking);
    }

    public void SetControlledVehicle(VehicleController vehicle)
    {
        controlledVehicle = vehicle;
    }
}
