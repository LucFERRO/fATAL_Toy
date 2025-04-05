using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPreview trajectoryPreview;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 50.0f)]
    float force;

    [SerializeField]
    Transform StartPosition;

    public InputAction fire;

    void OnEnable()
    {
        trajectoryPreview = GetComponent<TrajectoryPreview>();

        if (StartPosition == null)
            StartPosition = transform;

        fire.Enable();
        fire.performed += ThrowObject;
    }

    void Update()
    {
        Predict();
    }

    void Predict()
    {
        trajectoryPreview.PredictTrajectory(ProjectileData());
    }

    PhysicalDiceProperties ProjectileData()
    {
        PhysicalDiceProperties properties = new PhysicalDiceProperties();
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = force;
        properties.mass = r.mass;
        properties.drag = r.linearDamping;

        return properties;
    }

    void ThrowObject(InputAction.CallbackContext ctx)
    {
        Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);
    }
}
