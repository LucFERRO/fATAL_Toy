using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    #region Members
    LineRenderer trajectoryLine;
    [SerializeField, Tooltip("The marker will show where the projectile will hit")]
    Transform hitMarker;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
    int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
    float increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float rayOverlap = 1.1f;
    [SerializeField, Tooltip("The camera used to convert mouse position to world position")]
    Camera mainCamera;
    [SerializeField, Tooltip("The projectile properties to use for trajectory prediction")]
    PhysicalDiceProperties projectile;
    Vector3 lastMousePosition;
    private float mouseMovementThreshold = 0.1f;
    #endregion

    private void Start()
    {
        trajectoryLine = GetComponent<LineRenderer>();
        SetTrajectoryVisible(false);

        // Ensure the main camera is assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                //Debug.LogError("Main camera is not assigned and no Camera.main found. Please assign a camera.");
                enabled = false; // Disable the script to prevent further errors
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        // Only update trajectory if the mouse has moved
        //if (Vector3.Distance(Input.mousePosition, lastMousePosition) > mouseMovementThreshold)
        //{
            lastMousePosition = Input.mousePosition;
            UpdateTrajectoryToMousePosition();
        //}
    }

    private void UpdateTrajectoryToMousePosition()
    {
        if (!projectile.IsValid())
        {
            Debug.Log("Invalid projectile properties. Trajectory prediction aborted.");
            return;
        }

        // Raycast from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Set the initial position of the projectile to the dice's spawn position
            projectile.initialPosition = transform.position;

            // Update the direction to point from the initial position to the hit point
            projectile.direction = (hit.point - projectile.initialPosition).normalized;

            // Recalculate the trajectory
            PredictTrajectory();
        }
    }

    public void PredictTrajectory()
    {
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
        Vector3 position = projectile.initialPosition;
        Vector3 nextPosition;
        float overlap;

        trajectoryLine.positionCount = 1;
        trajectoryLine.SetPosition(0, position);

        for (int i = 1; i < maxPoints; i++)
        {
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                trajectoryLine.positionCount = i + 1;
                trajectoryLine.SetPosition(i, hit.point);
                MoveHitMarker(hit);
                return;
            }

            position = nextPosition;
            trajectoryLine.positionCount = i + 1;
            trajectoryLine.SetPosition(i, position);
        }

        hitMarker.gameObject.SetActive(false);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    private void MoveHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);

        float offset = 0.025f;
        hitMarker.position = hit.point + hit.normal * offset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }
    public void SetProjectileProperties(PhysicalDiceProperties properties)
    {
        projectile = properties;
    }
    public PhysicalDiceProperties GetProjectileProperties()
    {
        return projectile;
    }
}