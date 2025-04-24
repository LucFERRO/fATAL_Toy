using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPreview trajectoryPreview;
    PhysicalDiceProperties properties;
    Camera cam;
    [SerializeField, Tooltip("Offset for the throw origin relative to the player's view")]
    Vector3 throwOffset = new Vector3(0.5f, 0, 0); // Example: Offset slightly to the right


    private PhysicalDiceSpawner diceSpawner;
    private GameManager gameManager;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 200.0f)]
    float force;

    public Transform StartPosition;

    public InputAction fire;

    void OnEnable()
    {
        trajectoryPreview = GetComponent<TrajectoryPreview>();
        if (diceSpawner == null)
        {
            diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        }
        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        cam = Camera.main;

        if (StartPosition == null)
        {
            StartPosition = transform;
        }

        properties = InitializeProjectileProperties();

        // Pass the initialized properties to the TrajectoryPreview
        trajectoryPreview.SetProjectileProperties(properties);
        trajectoryPreview.SetThrowOffset(throwOffset);
        fire.Enable();
        fire.performed += ThrowObject;
    }

    private PhysicalDiceProperties InitializeProjectileProperties()
    {
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        // Apply the offset to the initial position
        Vector3 offsetPosition = StartPosition.position + cam.transform.right * throwOffset.x + cam.transform.up * throwOffset.y + cam.transform.forward * throwOffset.z;

        properties.direction = (offsetPosition - StartPosition.position).normalized;
        properties.initialPosition = offsetPosition;
        properties.initialSpeed = force;
        properties.mass = r.mass;
        properties.drag = r.linearDamping;

        return properties;
    }

    void Update()
    {
        trajectoryPreview.SetTrajectoryVisible(gameManager.isPreviewing);
        bool canThrowDice = gameManager.transform.childCount == 0;

        if (Input.GetMouseButton(0) && canThrowDice)
        {
            gameManager.isPreviewing = true;
        }
        if (Input.GetMouseButtonUp(0) && canThrowDice)
        {
            gameManager.isPreviewing = false;
            PhysicalDiceProperties updatedProperties = trajectoryPreview.GetProjectileProperties();

            // Spawn the dice at the camera's position and throw it towards the mouse position
            diceSpawner.SpawnDice(updatedProperties.direction * force, updatedProperties.initialPosition);
        }

        if (gameManager.isPreviewing)
        {
            Predict();
        }
    }

    void Predict()
    {
        trajectoryPreview.PredictTrajectory();
    }

    void ThrowObject(InputAction.CallbackContext context)
    {

    }
}