using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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

        if (Input.GetMouseButtonDown(0) && canThrowDice && !gameManager.isPreviewing && !EventSystem.current.IsPointerOverGameObject())

        {
            StartCoroutine(SetTestPreviewAfterDelay(0.1f));
        }

        if (gameManager.isPreviewing)
        {
            Predict();
            if (Input.GetMouseButtonDown(1))
            {
                gameManager.isPreviewing = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                PhysicalDiceProperties updatedProperties = trajectoryPreview.GetProjectileProperties();
                diceSpawner.SpawnDice(updatedProperties.direction * force, updatedProperties.initialPosition);
                gameManager.isPreviewing = false;
            }

        }
        trajectoryPreview.SetTrajectoryVisible(gameManager.isPreviewing);
    }

    private IEnumerator SetTestPreviewAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.isPreviewing = true;
    }

    void Predict()
    {
        trajectoryPreview.PredictTrajectory();
    }
    void ThrowObject(InputAction.CallbackContext context)
    {

    }
}