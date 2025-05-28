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
    private DefeatManager defeatManager;
    public bool isOverUI;

    private float nextAllowedThrowTime = 0f;
    [SerializeField] private float throwCooldown = 0.5f;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 200.0f)] float force;

    public Transform StartPosition;

    public InputAction fire;

    public SplineSwitcher splineSwitcher;

    private FMOD.Studio.EventInstance PreviewEventInstance;
    private FMOD.Studio.EventInstance DiceThrowEventInstance;

    void OnEnable()
    {
        trajectoryPreview = GetComponent<TrajectoryPreview>();
        //GlowingHexes.OnDiceDestroyed += HandleDiceDestroyed;
        if (diceSpawner == null)
        {
            diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        }
        GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerGO.GetComponent<GameManager>();
        uiManager = gameManagerGO.GetComponent<UiManager>();
        defeatManager = gameManagerGO.GetComponent<DefeatManager>();
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

        PreviewEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Preview");
        DiceThrowEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Throw");
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
        bool canThrowDice = gameManager.transform.childCount == 0 && !uiManager.isInventoryOpen && !splineSwitcher.isIdle && defeatManager.canRoll && Time.time >= nextAllowedThrowTime;
        if (Input.GetMouseButtonDown(0) && uiManager.isInventoryOpen && !isOverUI)
        {
            uiManager.IsInventoryOpen = !uiManager.IsInventoryOpen;
        }
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
                Cursor.visible = true;
                PreviewEventInstance.setParameterByName("PreviewState", 1);
                PreviewEventInstance.start();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (isOverUI)
                {
                    gameManager.isPreviewing = false;
                    Cursor.visible = true;
                    PreviewEventInstance.setParameterByName("PreviewState", 1);
                    PreviewEventInstance.start();
                    return;
                }
                DiceThrowEventInstance.start();
                Cursor.visible = true;
                PhysicalDiceProperties updatedProperties = trajectoryPreview.GetProjectileProperties();
                diceSpawner.SpawnDice(updatedProperties.direction * force, updatedProperties.initialPosition);
                gameManager.isPreviewing = false;
                if (!gameManager.diceWasThrown)
                {
                    gameManager.diceWasThrown = true;
                }
            }
        }
        trajectoryPreview.SetTrajectoryVisible(gameManager.isPreviewing);
    }

    //private void HandleDiceDestroyed()
    //{
    //    nextAllowedThrowTime = Time.time + throwCooldown;
    //}

    private IEnumerator SetTestPreviewAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.isPreviewing = true;
        Cursor.visible = false;
        PreviewEventInstance.setParameterByName("PreviewState", 0);
        PreviewEventInstance.start();
    }

    void Predict()
    {
        trajectoryPreview.PredictTrajectory();
    }
    void ThrowObject(InputAction.CallbackContext context)
    {

    }
}