using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PoliceLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private float checkpointRadius = 5f;

    [SerializeField] private AnimationCurve curveSpeedThreshold;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = false;

    [SerializeField] private float resetPathTime = 1f;
    private float resetPathTimer = 0f;

    private bool isStuck = false;
    private int timesStuck = 0;

    // Cache
    private VehicleController vehicle;
    private RaycastHit hit;

    private List<Vector3> path = new List<Vector3>();

    private void Awake()
    {
        vehicle = GetComponent<VehicleController>();
    }

    private void Update()
    {
        if (target == null)
            return;

        if (vehicle == null)
            return;

        float velocity_ratio = vehicle.GetVelocityRatio();
        if (velocity_ratio <= 0.1f)
        {
            resetPathTimer += Time.deltaTime;

            // Clear Path because we are stuck.
            if (resetPathTimer >= resetPathTime)
            {
                path.Clear();
                resetPathTimer = 0f;
                timesStuck++;

                if (timesStuck >= 3)
                {
                    isStuck = true;
                }
            }
        }
        else
        {
            resetPathTimer = 0f;
        }

        Vector3 target_direction = target.position - transform.position;
        Vector3 vehicle_forward = transform.forward;

        // If the Target is within the threshold, clear the Path and move towards them
        bool inView = TargetInView();

        // Generate a Path to the Target.
        if (!inView)
        {
            // Add to pathing.
            GeneratePath();

            // Smooth the Path.
            SmoothPath();
        }

        // Remove the next Path Point if it's within a certain distance.
        if (path.Count > 0 && Vector3.Distance(transform.position, path[0]) < checkpointRadius)
            path.RemoveAt(0);

        // Next path point direction.
        Vector3 next_path_point = (path.Count > 0) ? path[0] : target.position;
        Vector3 next_path_direction = next_path_point - transform.position;
        // Angle between the Vehicle Forward and the Next Path Point.
        float next_angle = Vector3.SignedAngle(vehicle_forward, next_path_direction, Vector3.up);
        // Get a direction from -1 to 1 to steer towards the next Path Point.
        float steering = Mathf.Clamp(next_angle / 90f, -1f, 1f);


        float acceleration = curveSpeedThreshold.Evaluate(velocity_ratio);
        bool is_braking = false;

        

        // If the angle from one point to the next is high, brake.
        // Also need high velocity.
        if (Mathf.Abs(next_angle) > 45f && velocity_ratio >= 0.5f)
            is_braking = true;

        // Check if next position is behind us.
        bool reversing = false;

        // If we have an object directly in front of us, reverse.
        //if (Physics.Raycast(transform.position, vehicle_forward, out hit, 5f, collisionMask))
        //    reversing = true;

        // If the next path point is behind us and we are moving forward, reverse.
        if (Vector3.Dot(vehicle_forward, next_path_direction) < 0f && velocity_ratio > 0.5f)
            reversing = true;

        if (reversing)
        {
            acceleration *= -1f;
            steering *= -1f;
        }
            

        // Steer towards the next Path Point. (reverse flips the steering sign).
        // Update Vehicle Input.

        vehicle.SetTurnInput(steering);
        vehicle.SetForwardInput(acceleration);
        vehicle.SetIsBraking(is_braking);


        if (showGizmos)
        {
            Debug.DrawRay(transform.position, target_direction / 2f, Color.red);
            Debug.DrawRay(transform.position, vehicle_forward * 10f, Color.blue);

            // Draw Path.
            if (path.Count > 1)
            {
                Debug.DrawLine(transform.position, path[0], Color.magenta);
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i], path[i + 1], Color.magenta);
                }
            }
        }
    }

    [Header("Pathfinding Settings")]
    [SerializeField] private int raycastAttempts = 3;
    [SerializeField] private float distanceThreshold = 5f;
    [SerializeField] private float rayAngle = 30f;
    [SerializeField] private float rayAngleIncrease = 10f;
    [SerializeField] private float minimumAddedDistance = 5f;

    private float distance_to_target = 0f;
    private int rayAttempts = 0;

    private bool TargetInView()
    {
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, distanceThreshold, targetMask))
        {
            // Clear Path
            path.Clear();
            return true;
        }
            
        return false;
    }

    private void GeneratePath()
    {
        Vector3 starting_position = (path.Count > 0) ? path[^1] : transform.position;

        // If the distance to the Target is less than the Threshold, return.
        if (Vector3.Distance(starting_position, target.position) < distanceThreshold)
            return;

        
        distance_to_target = Vector3.Distance(starting_position, target.position);

        // If the distance to the target is under the threshold, return.
        if (distance_to_target < distanceThreshold)
            return;

        Vector3 towards_target = (target.position - starting_position).normalized;

        // Attempt Raycast from my current position towards the Target (up to 3 Raycasts).
        List<Vector3> potential_positions = new List<Vector3>();

        for (int i = 0; i < raycastAttempts; i++)
        {
            // Try a random angle parallel to the ground.
            //float random_angle = Random.Range(-rayAngle, rayAngle) / 2 * rayAttempts;
            float random_angle = Random.Range(-180, 180);
            Vector3 random_direction = Quaternion.Euler(0f, random_angle, 0f) * towards_target;

            if (Physics.Raycast(starting_position, random_direction, out hit, distance_to_target, collisionMask))
            {
                // The Halfway Point.
                Vector3 halfway_point = starting_position + random_direction * hit.distance / 2f;
                potential_positions.Add(halfway_point);
                rayAttempts = 0;
            }
            else
            {
                Vector3 halfway_point = starting_position + random_direction * distance_to_target / 2f;
                potential_positions.Add(halfway_point);
                rayAttempts++;
            }
        }

        // Check each of the hits.
        // Pick the one closest to the Target.
        Vector3 closest_position = potential_positions[0];
        float closest_distance = Vector3.Distance(closest_position, target.position);
        
        for (int i = 1; i < potential_positions.Count; i++)
        {
            float distance = Vector3.Distance(potential_positions[i], target.position);
            if (distance < closest_distance)
            {
                closest_position = potential_positions[i];
                closest_distance = distance;
            }
        }

        // Escape if the distance is less than distance threshold.
        if (closest_distance < distanceThreshold)
            return;

        // Only add the closest hit if it is closer than the current starting position.
        if (Vector3.Distance(starting_position, target.position) < closest_distance)
            return;

        // Only add if the distance is more than the minimum added distance.
        if (Vector3.Distance(starting_position, closest_position) < minimumAddedDistance)
            return;

        // Add the closest hit to the Path, but bring it back 50% of the way.
        path.Add(closest_position);
    }

    private void SmoothPath()
    {
        if (path.Count <= 2)
            return;

        // 50% Chance to remove first point
        if (Random.Range(0, 2) == 0)
            RemoveFirstPathPoint();
        else
        {
            // Get a random point in the Path.
            int i = Random.Range(0, path.Count - 2);
            RemovePathIndex(i);
        }
    }

    private void RemoveFirstPathPoint()
    {
        if (!Physics.Raycast(transform.position, path[1] - transform.position, out hit, Vector3.Distance(transform.position, path[1]), collisionMask))
        {
            path.RemoveAt(0);
        }
    }

    private void RemovePathIndex(int index)
    {
        Vector3 first_point = path[index];
        Vector3 last_point = path[index + 2];

        if (!Physics.Raycast(first_point, last_point - first_point, out hit, Vector3.Distance(first_point, last_point), collisionMask))
        {
            path.RemoveAt(index + 1);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public bool IsStuck()
    {
        return isStuck;
    }

    public void ResetIsStuck()
    {
        isStuck = false;
        timesStuck = 0;
        path.Clear();
    }

    public float DistanceToTarget()
    {
        return distance_to_target;
    }
}
