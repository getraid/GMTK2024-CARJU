using UnityEngine;

public class CarTerrainConstraints : MonoBehaviour
{
    public Terrain terrain;
    public float minValidHeight = 0.5f;
    public float maxValidHeight = 5f;
    public float raycastDistance = 1f;
    public LayerMask groundLayer;

    private void Update()
    {
        // Get the current position of the car
        Vector3 carPosition = transform.position;

        // Sample the terrain height at the car's position
        float terrainHeight = terrain.SampleHeight(carPosition);

        // Check if the car is within the valid height range
        if (carPosition.y < terrainHeight + minValidHeight || carPosition.y > terrainHeight + maxValidHeight)
        {
            // Adjust the car's position to stay within the valid height range
            carPosition.y = Mathf.Clamp(carPosition.y, terrainHeight + minValidHeight, terrainHeight + maxValidHeight);
            transform.position = carPosition;
        }

        // Perform a downward raycast to ensure the car stays on the terrain surface
        RaycastHit hit;
        if (Physics.Raycast(carPosition, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // Move the car to the hit point to stay on the terrain surface
            transform.position = hit.point;
        }
    }
}
