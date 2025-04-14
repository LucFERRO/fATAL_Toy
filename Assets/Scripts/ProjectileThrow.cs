using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPreview trajectoryPreview;
    Camera cam;

    private PhysicalDiceSpawner diceSpawner;
    private GameManager gameManager;

    [SerializeField]
    Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 200.0f)]
    float force;

    [SerializeField]
    Transform StartPosition;

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

        fire.Enable();
        fire.performed += ThrowObject;
    }

    void Update()
    {
        trajectoryPreview.SetTrajectoryVisible(gameManager.isPreviewing);
        bool canThrowDice = gameManager.transform.childCount == 0;

        if (Input.GetMouseButton(0) && canThrowDice)
        {
            gameManager.isPreviewing = true;
            //diceSpawner.SpawnDice(transform.position - cam.transform.position, cam.transform);
        }
        if (Input.GetMouseButtonUp(0) && canThrowDice)
        {
            gameManager.isPreviewing = false;
            diceSpawner.SpawnDice(StartPosition.forward * force, transform);
        }

        if (gameManager.isPreviewing)
        {
            Predict();
        }
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
        //Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        //thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);


        //Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //if (Input.GetMouseButtonUp(1))
        //{
        //diceSpawner.SpawnDice(StartPosition.forward * force, transform);
        //trajectoryPreview.SetTrajectoryVisible(false);
        //}
        //}
    }
}
