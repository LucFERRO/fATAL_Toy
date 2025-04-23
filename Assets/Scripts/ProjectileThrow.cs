using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPreview trajectoryPreview;
    PhysicalDiceProperties properties;
    Camera cam;

    private PhysicalDiceSpawner diceSpawner;
    private GameManager gameManager;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 200.0f)]
    float force;

    public Transform StartPosition { get; private set; }

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

        fire.Enable();
        fire.performed += ThrowObject;
    }

    private PhysicalDiceProperties InitializeProjectileProperties()
    {
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();
        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
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
            Debug.Log(updatedProperties.initialPosition);
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