using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] tireAnchors = new Transform[4];
    
    [SerializeField] private TrailRenderer skidMarksVFX;
    [SerializeField] private ParticleSystem tireSmokeVFX;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource skidSound;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask drivableMask;

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
    [SerializeField] private float tireForce = 1f;
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
    [SerializeField] private float skidMarkWidth = 0.22f;
    [SerializeField] private bool testSkidMarkWidthOnPlay = false;
    [SerializeField] private AnimationCurve particleSpeedThreshold;
    [SerializeField] private AnimationCurve particleTurnThreshold;

    [Header("Sound")]
    [SerializeField] private Vector2 pitchRange = new Vector2(1f, 5f);

    // Components
    private Rigidbody _rigidbody;

    // States
    private float[] _wheelGroundedDistance = new float[4];
    private bool _isGrounded = false;

    // Inputs
    private float _forwardInput = 0;
    private float _reverseInput = 0;
    private float _steerInput = 0;
    private bool _isBraking = false;

    // Calculations
    private Vector3 _currentLocalVelocity = Vector3.zero;
    private float _velocityRatio = 0f;
    private float _currentSteeringAngle = 0f;

    // Containers
    private Transform _partsContainer;
    private Transform[] _tireVisuals = new Transform[4];
    private List<TrailRenderer> _skidMarkContainer = new List<TrailRenderer>();
    private List<ParticleSystem> _tireSmokeContainer = new List<ParticleSystem>();

    // Cache
    RaycastHit hit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _partsContainer = new GameObject("Parts Container").transform;
        _partsContainer.SetParent(transform);

        for (int i = 0; i < _tireVisuals.Length; i++)
        {
            // Set the tire visuals
            _tireVisuals[i] = tireAnchors[i].GetChild(0);

            // Skid Marks need to spawn with 90x rotation
            _skidMarkContainer.Add(Instantiate(skidMarksVFX, _tireVisuals[i].position, Quaternion.Euler(90f, 0f, 0f), _partsContainer));
            _skidMarkContainer[i].startWidth = skidMarkWidth;

            // Tire Smoke VFX
            _tireSmokeContainer.Add(Instantiate(tireSmokeVFX, _tireVisuals[i].position, Quaternion.identity, _partsContainer));
        }
    }

    private void Update()
    {
        PlayVFX();
        EngineSound();
        UpdateSpeedometer();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();

        UpsideDownCheck();
    }
    

    private void GroundCheck()
    {
        for (int i = 0; i < _wheelGroundedDistance.Length; i++)
        {
            if (_wheelGroundedDistance[i] > 0f)
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
            DownForce();
        }
    }

    public Rigidbody GetRigidBody()
    {
        return _rigidbody;
    }

    [Header("Upside Down Check")]
    [SerializeField] private bool isFlippable = false;
    [SerializeField] private float roughHeight = 10f;
    [SerializeField] private float flipVelocityTheshold = 5f;
    [SerializeField] private float flipTimer = 3f;
    private float _upsideDownTimer = 0f;

    private void UpsideDownCheck()
    {
        if (!isFlippable)
            return;

        // Check velocity
        if (_rigidbody.velocity.magnitude > flipVelocityTheshold)
            return;

        // Raycast Above the Vehicle
        if (Physics.Raycast(transform.position, transform.up, out hit, roughHeight, drivableMask))
        {
            _upsideDownTimer += Time.fixedDeltaTime;

            if (_upsideDownTimer >= flipTimer)
            {
                _upsideDownTimer = 0f;
                transform.position += Vector3.up * roughHeight;
                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }
        }
        else
        {
            _upsideDownTimer = 0f;
        }

        if (showGizmos)
        {
            Debug.DrawLine(transform.position, transform.position + transform.up * roughHeight, Color.magenta);
        }
    }

    public void TransferRigidBodyParameters(Rigidbody rb)
    {
        _rigidbody.velocity = rb.velocity;
        _rigidbody.angularVelocity = rb.angularVelocity;
        _rigidbody.angularDrag = rb.angularDrag;
    }

    #region Physics Applications
    private void Suspension()
    {
        float max_distance = restLength + springTravel;
        for (int i = 0; i < tireAnchors.Length; i++)
        {
            if (Physics.Raycast(tireAnchors[i].position, -tireAnchors[i].up, out hit, max_distance + wheelRadius, drivableMask))
            {
                _wheelGroundedDistance[i] = hit.distance;

                float current_spring_length = hit.distance - wheelRadius;
                float spring_compression = (restLength - current_spring_length) / springTravel;

                float spring_velocity = Vector3.Dot(_rigidbody.GetPointVelocity(tireAnchors[i].position), tireAnchors[i].up);
                float damp_force = damperStiffness * spring_velocity;

                float spring_force = springStiffness * spring_compression;

                float net_force = spring_force - damp_force;

                _rigidbody.AddForceAtPosition(net_force * tireAnchors[i].up, tireAnchors[i].position);

                // Visuals
                SetTirePosition(_tireVisuals[i], hit.point + wheelRadius * tireAnchors[i].up);

                // Helper
                if (showGizmos)
                {
                    // Draw spring force and damp force
                    Debug.DrawLine(tireAnchors[i].position, tireAnchors[i].position + spring_force * 0.01f * tireAnchors[i].up, Color.red);
                    Debug.DrawLine(tireAnchors[i].position, tireAnchors[i].position + damp_force * 0.01f * tireAnchors[i].up, Color.blue);
                }
            }
            else
            {
                _wheelGroundedDistance[i] = 0f;

                // Visuals
                SetTirePosition(_tireVisuals[i], tireAnchors[i].position - tireAnchors[i].up * max_distance);

                // Helper
                if (showGizmos)
                {
                    Debug.DrawLine(tireAnchors[i].position, tireAnchors[i].position + (wheelRadius + max_distance) * -tireAnchors[i].up, Color.green);
                }
            }
        }
    }

    private void Turn()
    {
        float steering_angle = maxSteeringAngle * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio));
        _currentSteeringAngle = Mathf.MoveTowards(_currentSteeringAngle, steering_angle, steerStrength * Time.fixedDeltaTime);
    }

    private void WheelAcceleration()
    {
        // Apply Force at the Tire Positions based on their rotation
        for (int i = 0; i < _tireVisuals.Length; i++)
        {
            // Cancel if the tire is not grounded
            if (_wheelGroundedDistance[i] <= 0f)
                continue;

            Transform tire = _tireVisuals[i];

            if (i < 2) // Only apply for the front 2 tires
            {
                // Setup Forces
                Vector3 steering_forward = Quaternion.Euler(0f, _currentSteeringAngle, 0f) * tireAnchors[i].forward;
                Vector3 forward_force = _forwardInput * acceleration * accelerationCurve.Evaluate(Mathf.Abs(_velocityRatio)) / 2f * steering_forward;
                Vector3 backward_force = _reverseInput * reverse * accelerationCurve.Evaluate(Mathf.Abs(_velocityRatio)) / 2f * -transform.forward;
                Vector3 brake_force = (_isBraking) ? -_currentLocalVelocity.z * brakeForce * Time.fixedDeltaTime * transform.forward : Vector3.zero;

                // If no forward or backward input, apply full brake force
                if (_isBraking && _forwardInput == 0f && _reverseInput == 0f)
                {
                    brake_force = -_currentLocalVelocity.z * brakeForce * transform.forward;
                }

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
                    Debug.DrawLine(tire.position, tire.position + forward_force, Color.green);
                    Debug.DrawLine(tire.position, tire.position + backward_force, Color.yellow);
                    Debug.DrawLine(tire.position, tire.position + brake_force, Color.cyan);
                }
            }
            else // Only apply to the back 2 tires
            {
                // Spin the wheel
                tire.transform.Rotate(Vector3.right, tireRotationSpeed * _velocityRatio * Time.deltaTime, Space.Self);
            }
        }
    }

    // Apply Downward Force at the Tire Positions
    private void DownForce()
    {
        for (int i = 0; i < tireAnchors.Length; i++)
        {
            Vector3 force = tireMass * Physics.gravity;
            _rigidbody.AddForceAtPosition(force, tireAnchors[i].position, ForceMode.Acceleration);

            if (showGizmos)
            {
                Debug.DrawLine(tireAnchors[i].position, tireAnchors[i].position + force, Color.blue);
            }
        }
    }

    private void SidewaysDrag()
    {
        // Apply Force at the Tire Positions based on their rotation
        for (int i = 0; i < _tireVisuals.Length; i++)
        {
            // Cancel if the tire is not grounded
            if (_wheelGroundedDistance[i] <= 0f)
                continue;

            Transform tire = _tireVisuals[i];

            // Apply forward force at the position of the tire in its rotation
            Vector3 steering_direction = tire.right;
            Vector3 tire_world_velocity = _rigidbody.GetPointVelocity(tire.position);
            float steering_velocity = Vector3.Dot(steering_direction, tire_world_velocity);
            float desired_velocity_change = -steering_velocity * tireGripFactor;
            float desired_acceleration = desired_velocity_change / Time.fixedDeltaTime;

            Vector3 force_applied = desired_acceleration * tireForce * steering_direction;
            
            if (_isBraking)
            {
                force_applied = gripCurve.Evaluate(Mathf.Abs(_velocityRatio)) * tireGripFactor * -tire_world_velocity;
            }

            // If no forward or backward input, double the grip
            if (_isBraking && _forwardInput == 0f && _reverseInput == 0f)
            {
                force_applied *= 2f;
            }

            _rigidbody.AddForceAtPosition(force_applied, _tireVisuals[i].transform.position, ForceMode.Acceleration);

            if (showGizmos)
            {
                Debug.DrawLine(tire.position, tire.position + force_applied, Color.red);
            }
        }
    }
    #endregion

    #region Visuals
    private void SetTirePosition(Transform tire, Vector3 position)
    {
        tire.transform.position = position;
    }

    private void PlayVFX()
    {
        bool play_skid_sound = false;
        float speed_threshold = particleSpeedThreshold.Evaluate(Mathf.Abs(_velocityRatio));

        for (int i = 0; i < _tireVisuals.Length; i++)
        {
            if (testSkidMarkWidthOnPlay)
            {
                _skidMarkContainer[i].startWidth = skidMarkWidth;
            }

            // Set the Position just above the ground
            _skidMarkContainer[i].transform.position = tireAnchors[i].position + Vector3.down * _wheelGroundedDistance[i] + Vector3.up * 0.1f;
            _tireSmokeContainer[i].transform.position = tireAnchors[i].position + Vector3.down * _wheelGroundedDistance[i] + Vector3.up * 0.1f;

            // Automatically mark as inactive.
            _skidMarkContainer[i].emitting = false;

            // Cancel if the tire is not grounded
            if (_wheelGroundedDistance[i] <= 0f)
                continue;

            // We want to get a float based on the tire's facing direction vs the vehicle's velocity
            Vector3 tire_forward = (i < 2) ? Quaternion.Euler(0f, _currentSteeringAngle, 0f) * tireAnchors[i].forward : tireAnchors[i].forward;
            Vector3 vehicle_forward = transform.forward;
            float offset_angle = Vector3.Angle(tire_forward, vehicle_forward);

            float turn_threshold = particleTurnThreshold.Evaluate(offset_angle / maxSteeringAngle);
            

            bool is_skidding = false;
            if (_isBraking && speed_threshold > 0.1f)
            {
                is_skidding = true;
            }
            else if (speed_threshold >= 1f && turn_threshold >= 1f)
            {
                is_skidding = true;
            }

            // Emit the VFX and Sound
            if (is_skidding)
            {
                play_skid_sound = true;
                _skidMarkContainer[i].emitting = true;
                _tireSmokeContainer[i].Emit(Mathf.RoundToInt(turn_threshold + speed_threshold));

                // Check if tire smoke has any children particles and emit those.
                for (int j = 0; j < _tireSmokeContainer[i].transform.childCount; j++)
                {
                    _tireSmokeContainer[i].transform.GetChild(j).GetComponent<ParticleSystem>().Emit(Mathf.RoundToInt(turn_threshold + speed_threshold));
                }
                
            }
        }

        if (play_skid_sound)
        {
            ToggleSkidSound(true, speed_threshold);
        }
        else
        {
            ToggleSkidSound(false, 0f);
        }
    }

    float speedometerPercent = 0f;
    float speedometerPercentLerp = 0.50f;

    private void UpdateSpeedometer()
    {
        if (GameManager.Instance == null)
            return;

        float velocity = Mathf.Abs(_velocityRatio);
        float level_modifier = (float)GameManager.Instance.CurrentPlayerLevel / 5f; // Percentage based on Car Level
        float factor = 0.90f; // 90% of the speedometer is the actual speed

        speedometerPercent = Mathf.Lerp(speedometerPercent, velocity * level_modifier * factor, Time.deltaTime * speedometerPercentLerp);

        GameManager.Instance.SpeedometerPercentGUI = speedometerPercent;
    }
    #endregion

    #region Input Methods
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
    #endregion

    #region Audio
    private void EngineSound()
    {
        if (engineSound == null)
            return;

        engineSound.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, Mathf.Abs(_velocityRatio));
    }

    private void ToggleSkidSound(bool isPlaying, float volume)
    {
        if (skidSound == null)
            return;

        
        skidSound.mute = !isPlaying;
        skidSound.volume = 0.1f * (1f + volume);
    }

    #endregion

    #region Public Methods
    public float GetVelocityRatio()
    {
        return _velocityRatio;
    }

    #endregion

}
