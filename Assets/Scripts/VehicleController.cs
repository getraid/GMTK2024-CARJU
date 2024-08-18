using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private Transform[] tires = new Transform[4];

    [Header("Suspension")]
    [SerializeField] private float springStiffness = 30000f;
    [SerializeField] private float damperStiffness = 3000f;
    [SerializeField] private float restLength = 0.1f;
    [SerializeField] private float springTravel = 0.4f;
    [SerializeField] private float wheelRadius = 0.5f;

    [Header("Stearing")]
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private AnimationCurve turningCurve;

    [Header("Tire Friction")]
    [SerializeField] private float tireMass = 1f;
    [SerializeField] private float tireGripFactor = 1f;
    [SerializeField] private AnimationCurve gripCurve;

    [Header("Speed")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float reverse = 10f;
    [SerializeField] private float brakeForce = 10f;
    [SerializeField] private AnimationCurve accelerationCurve;

    [Header("Visuals")]
    [SerializeField] private bool showGizmos = false;
    [SerializeField] private float tireRotationSpeed = 30f;
   

    private Rigidbody _rigidbody;

    private int[] _wheelsGrounded = new int[4];
    private bool _isGrounded = false;

    private float _forwardInput = 0;
    private float _reverseInput = 0;
    private float _steerInput = 0;
    private bool _isBraking = false;

    private Vector3 _currentLocalVelocity = Vector3.zero;
    private float _velocityRatio = 0f;

    private float _currentSteeringAngle = 0f;

    // Cache
    RaycastHit hit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    // This works.
    private void Suspension()
    {
        float max_distance = restLength + springTravel;
        for (int i = 0; i < rayPoints.Length; i++)
        {
            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, max_distance + wheelRadius))
            {
                _wheelsGrounded[i] = 1;

                float current_spring_length = hit.distance - wheelRadius;
                float spring_compression = (restLength - current_spring_length) / springTravel;

                float spring_velocity = Vector3.Dot(_rigidbody.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float damp_force = damperStiffness * spring_velocity;

                float spring_force = springStiffness * spring_compression;

                float net_force = spring_force - damp_force;

                _rigidbody.AddForceAtPosition(net_force * rayPoints[i].up, rayPoints[i].position);

                // Visuals
                SetTirePosition(tires[i], hit.point + wheelRadius * rayPoints[i].up);

                // Helper
                if (showGizmos)
                {
                    Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
                }
            }
            else
            {
                _wheelsGrounded[i] = 0;

                // Visuals
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * max_distance);

                // Helper
                if (showGizmos)
                {
                    Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + max_distance) * -rayPoints[i].up, Color.green);
                }
            }
        }
    }

    private void GroundCheck()
    {
        for (int i = 0; i < _wheelsGrounded.Length; i++)
        {
            if (_wheelsGrounded[i] == 1)
            {
                _isGrounded = true;
                return;
            }
        }

        _isGrounded = false;
    }

    private void CalculateCarVelocity()
    {
        _currentLocalVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
        _velocityRatio = _currentLocalVelocity.z / maxSpeed;
    }

    private void Movement()
    {
        if (_isGrounded)
        {
            Turn();
            SidewaysDrag();
            WheelAcceleration();
            
        }
    }

    private void Turn()
    {
        //Vector3 torque = steerStrength * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio)) * Mathf.Sign(_velocityRatio) * _rigidbody.transform.up;
        //_rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        // Turn the Tires
        float steering_angle = maxSteeringAngle * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio));
        _currentSteeringAngle = Mathf.MoveTowards(_currentSteeringAngle, steering_angle, steerStrength * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Why you no turn properly?
    /// </summary>
    private void WheelAcceleration()
    {
        // Apply Force at the Tire Positions based on their rotation
        for (int i = 0; i < tires.Length; i++)
        {
            // Cancel if the tire is not grounded
            if (_wheelsGrounded[i] == 0)
                continue;

            Transform tire = tires[i];

            if (i < 2) // Only apply for the front 2 tires
            {
                // Setup Forces
                Vector3 forward_force = _forwardInput * acceleration * accelerationCurve.Evaluate(Mathf.Abs(_velocityRatio)) / 2f * transform.forward;
                Vector3 backward_force = _reverseInput * reverse * accelerationCurve.Evaluate(Mathf.Abs(_velocityRatio)) / 2f * -transform.forward;
                Vector3 brake_force = (_isBraking) ? -_currentLocalVelocity.z * brakeForce * Time.fixedDeltaTime * transform.forward : Vector3.zero;

                // Turn the Tire
                Vector3 current_rotation = tire.transform.localEulerAngles;
                current_rotation.y = _currentSteeringAngle;
                current_rotation.z = 0f;

                tire.transform.localEulerAngles = current_rotation;

                // Spin the wheel
                tire.transform.Rotate(
                    Vector3.right,
                    (_isBraking) ? 0f : tireRotationSpeed * _velocityRatio * Time.deltaTime,
                    Space.Self
                );


                _rigidbody.AddForceAtPosition(forward_force + backward_force + brake_force, tire.transform.position, ForceMode.Acceleration);

                if (showGizmos)
                {
                    Debug.DrawLine(tire.position, tire.position + forward_force, Color.cyan);
                }
            }
            else // Only apply to the back 2 tires
            {
                // Spin the wheel
                tire.transform.Rotate(Vector3.right, tireRotationSpeed * _velocityRatio * Time.deltaTime, Space.Self);
            }
        }
    }



    private void SidewaysDrag()
    {
        // Apply Force at the Tire Positions based on their rotation
        for (int i = 0; i < tires.Length; i++)
        {
            // Cancel if the tire is not grounded
            if (_wheelsGrounded[i] == 0)
                continue;

            Transform tire = tires[i];

            // Apply forward force at the position of the tire in its rotation
            Vector3 steering_direction = tire.right;
            Vector3 tire_world_velocity = _rigidbody.GetPointVelocity(tire.position);
            float steering_velocity = Vector3.Dot(steering_direction, tire_world_velocity);
            float desired_velocity_change = -steering_velocity * tireGripFactor;
            float desired_acceleration = desired_velocity_change / Time.fixedDeltaTime;

            Vector3 force_applied = desired_acceleration * tireMass * steering_direction;
            
            if (_isBraking)
            {
                force_applied = -tire_world_velocity * tireGripFactor * gripCurve.Evaluate(Mathf.Abs(_velocityRatio));
            }

            _rigidbody.AddForceAtPosition(force_applied, tires[i].transform.position, ForceMode.Acceleration);

            if (showGizmos)
            {
                Debug.DrawLine(steering_direction, steering_direction * 10f, Color.blue);
                Debug.DrawLine(tire.position, tire.position + force_applied, Color.red);
            }
        }
    }

    private void SetTirePosition(Transform tire, Vector3 position)
    {
        tire.transform.position = position;
    }

    // Collecting Input

    public void SetForwardInput(float value)
    {
        _forwardInput = value;
    }

    public void SetReverseInput(float value)
    {
        _reverseInput = value;
    }

    public void SetTurnInput(float value)
    {
        _steerInput = value;
    }

    public void SetIsBraking(bool isBraking)
    {
       _isBraking = isBraking;
    }
}
