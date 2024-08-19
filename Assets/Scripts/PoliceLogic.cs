using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private LayerMask raycastMask;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = false;

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

        Vector3 target_direction = target.position - transform.position;
        Vector3 vehicle_forward = transform.forward;

        // Generate a Path to the Target.
        GeneratePath();

        // Remove the next Path Point if it's within a certain distance.
        if (path.Count > 0 && Vector3.Distance(transform.position, path[0]) < 2f)
            path.RemoveAt(0);

        // Check forwards/backwards.
        // Steer towards the next Path Point. (reverse flips the steering sign).
        // Update Vehicle Input.

        //vehicle.SetTurnInput();
        //vehicle.SetForwardInput();
        //vehicle.SetIsBraking();


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

    [SerializeField] private int raycastAttempts = 3;
    [SerializeField] private float distanceThreshold = 5f;

    private void GeneratePath()
    {
        // If the distance to the Target is less than the Threshold, return.
        if (Vector3.Distance(transform.position, target.position) < distanceThreshold)
            return;

        Vector3 starting_position = (path.Count > 0) ? path[path.Count - 1] : transform.position;
        float distance_to_target = Vector3.Distance(starting_position, target.position);

        // Attempt Raycast from my current position towards the Target (up to 3 Raycasts).
        List<Vector3> potential_positions = new List<Vector3>();

        for (int i = 0; i < raycastAttempts; i++)
        {
            // Try a random angle parallel to the ground.
            Vector3 random_direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            if (Physics.Raycast(starting_position, random_direction, out hit, distance_to_target, raycastMask))
            {
                // The Halfway Point.
                Vector3 halfway_point = starting_position + random_direction * hit.distance / 2f;
                potential_positions.Add(halfway_point);
            }
            else
            {
                Vector3 halfway_point = starting_position + random_direction * distance_to_target / 2f;
                potential_positions.Add(halfway_point);
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

        // Add the closest hit to the Path, but bring it back 50% of the way.
        path.Add(closest_position);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
