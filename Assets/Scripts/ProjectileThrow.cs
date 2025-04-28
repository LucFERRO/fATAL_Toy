using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPreview trajectoryPreview;
    PhysicalDiceProperties properties;
    Camera cam;
    public Vector2 screenSpaceOffset; // Example: Slightly right and down

    public GameObject bottomPlane;

    private PhysicalDiceSpawner diceSpawner;
    private GameManager gameManager;
    private UiManager uiManager;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 200.0f)] float force;

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
        if (uiManager == null)
        {
            uiManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UiManager>();
        }
        cam = Camera.main;

        if (StartPosition == null)
        {
            StartPosition = transform;
        }

        properties = InitializeProjectileProperties();

        // Pass the initialized properties to the TrajectoryPreview
        trajectoryPreview.SetProjectileProperties(properties);
        trajectoryPreview.SetScreenSpaceOffset(screenSpaceOffset);
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
        bool canThrowDice = gameManager.transform.childCount == 0 && !uiManager.isInventoryOpen;

        if (Input.GetMouseButton(0) && canThrowDice)
        {
            gameManager.isPreviewing = true;
        }
        if (Input.GetMouseButtonUp(0) && canThrowDice && gameManager.isPreviewing)
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
        trajectoryPreview.SetTrajectoryVisible(gameManager.isPreviewing);
    }

    void Predict()
    {
        trajectoryPreview.PredictTrajectory();
    }

    void ThrowObject(InputAction.CallbackContext context)
    {

    }
}