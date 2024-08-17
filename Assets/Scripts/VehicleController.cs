using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private Transform[] tires = new Transform[4];
    [SerializeField] private Transform[] frontTireParents = new Transform[2];
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension")]
    [SerializeField] private float springStiffness = 30000f;
    [SerializeField] private float damperStiffness = 3000f;
    [SerializeField] private float restLength = 0.1f;
    [SerializeField] private float springTravel = 0.4f;
    [SerializeField] private float wheelRadius = 0.5f;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private float dragCoefficient = 1f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private AnimationCurve accelerationCurve;

    [Header("Visuals")]
    [SerializeField] private bool showGizmos = false;
    [SerializeField] private float tireRotationSpeed = 30f;
    [SerializeField] private float maxSteeringAngle= 30f;


    private Rigidbody _rigidbody;

    private int[] _wheelsGrounded = new int[4];
    private bool _isGrounded = false;

    private float _moveInput = 0;
    private float _steerInput = 0;

    private Vector3 _currentLocalVelocity = Vector3.zero;
    private float _velocityRatio = 0f;

    private float _currentSteeringAngle = 0f;

    // Cache
    RaycastHit hit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //GetPlayerInput();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        //Visuals();
    }

    private void GetPlayerInput()
    {
        _moveInput = Input.GetAxisRaw("Vertical");
        _steerInput = Input.GetAxisRaw("Horizontal");
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
        // Turn the Tires
        float steering_angle = maxSteeringAngle * _steerInput;
        _currentSteeringAngle = Mathf.MoveTowards(_currentSteeringAngle, steering_angle, steerStrength * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Why you no turn properly?
    /// </summary>
    private void WheelAcceleration()
    {
        //Vector3 torque = steerStrength * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio)) * Mathf.Sign(_velocityRatio) * _rigidbody.transform.up;
        //_rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        // Apply Force at the Tire Positions based on their rotation
        for (int i = 0; i < tires.Length; i++)
        {
            // Cancel if the tire is not grounded
            if (_wheelsGrounded[i] == 0)
                continue;

            Transform tire = tires[i];

            if (i < 2) // Only apply for the front 2 tires
            {
                // Spin the wheel
                tire.transform.Rotate(Vector3.right, tireRotationSpeed * _velocityRatio * Time.deltaTime, Space.Self);

                // Rotate the tire visual
                frontTireParents[i].transform.localEulerAngles = new Vector3(frontTireParents[i].transform.localEulerAngles.x, _currentSteeringAngle, frontTireParents[i].transform.localEulerAngles.z);

                Vector3 forward = _moveInput * acceleration * accelerationCurve.Evaluate(Mathf.Abs(_velocityRatio)) / 2f * transform.forward;
                _rigidbody.AddForceAtPosition(forward, tires[i].transform.position, ForceMode.Acceleration);

                if (showGizmos)
                {
                    Debug.DrawLine(tire.position, tire.position + forward, Color.cyan);
                }
            }
            else // Only apply to the back 2 tires
            {
                // Spin the wheel
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * _moveInput * Time.deltaTime, Space.Self);

                // Apply forward force at the position of the tire in its rotation
                //_rigidbody.AddForceAtPosition(transform.forward * acceleration * _moveInput, tires[i].transform.position, ForceMode.Acceleration);
            }
        }
    }

    [SerializeField] private float tireGripFactor = 1f;
    [SerializeField] private float tireMass = 1f;


    private void SidewaysDrag()
    {
        // Old Implementation
        //float current_sideways_speed = _currentLocalVelocity.x;
        //float drag_magnitude = -current_sideways_speed * dragCoefficient;
        //Vector3 drag_force = transform.right * drag_magnitude;

        //_rigidbody.AddForceAtPosition(drag_force, _rigidbody.centerOfMass, ForceMode.Acceleration);

        // New Implementation

        // Force = Mass * Acceleration
        // Acceleration = Chance in Velocity / Time

        // Convert _currentSteeringAngle to a Vector3
        //Vector3 steering_direction = frontTireParents[0].transform.right;

        //float steering_velocity = Vector3.Dot(steering_direction, _rigidbody.velocity);

        //float desired_velocity_change = -steering_velocity * tireGripFactor;



        //// Apply Force at the Tire Positions based on their rotation
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

    private void Visuals()
    {
        TireVisuals();
    }

    private void TireVisuals()
    {
        float steering_angle = maxSteeringAngle * _steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2) // Only the front 2 tires
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * _velocityRatio * Time.deltaTime, Space.Self);

                frontTireParents[i].transform.localEulerAngles = new Vector3(frontTireParents[i].transform.localEulerAngles.x, steering_angle, frontTireParents[i].transform.localEulerAngles.z);
            }
            else // Only the back 2 tires
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * _moveInput * Time.deltaTime, Space.Self);
            }
        }
    }

    public void SetForwardInput(float input)
    {
        _moveInput = input;
    }

    public void SetTurnInput(float input)
    {
        _steerInput = input;
    }
}
