using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] VehicleController controlledVehicle;


    private Vector2 _inputAxis;


    void Update()
    {
        _inputAxis.x = Input.GetAxisRaw("Horizontal");
        _inputAxis.y = Input.GetAxisRaw("Vertical");

        if (controlledVehicle != null)
        {
            ApplyInputToVehicle();
        }
    }

    void ApplyInputToVehicle()
    {
        controlledVehicle.SetForwardInput(_inputAxis.y);
        controlledVehicle.SetTurnInput(_inputAxis.x);
    }
}
