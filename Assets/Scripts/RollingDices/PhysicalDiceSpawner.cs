using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalDiceSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameManagerGO;
    private GameManager gameManager;
    public GameObject diceToSpawn;

    //public RollingDiceManager diceManager;
    public int movementSpeed;
    public int spinForce;
    public int throwForce;
    public float distance;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    public float positionSwitchTimer;
    private float timer;
    void Start()
    {
        gameManager = gameManagerGO.GetComponent<GameManager>();
        startingPosition = transform.position;
        timer = positionSwitchTimer;
        // x 1.5 - 11.5
        // z -4 - 2
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = positionSwitchTimer;
            targetPosition = new Vector3(Random.Range(-distance, distance), 0, Random.Range(-distance, distance)) + startingPosition;
        }
        transform.position = Vector3.Slerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnDice(Vector3.zero, transform);
        }
    }

    public void SpawnDice(Vector3 throwVector, Transform originTransform)
    {
        GameObject spawnedDice = Instantiate(diceToSpawn, originTransform.position, originTransform.rotation);
        spawnedDice.transform.parent = gameManagerGO.transform;

        for (int i = 0; i < gameManager.diceFaces.Length; i++)
        {
            Transform face = spawnedDice.transform.GetChild(i);
            int biomeId = gameManager.diceFaces[i].transform.GetChild(0).GetComponent<DraggableItem>().biomeId;
            face.GetComponent<FaceComponent>().faceType = gameManager.tileTypes[biomeId];
            face.GetComponent<MeshRenderer>().material = gameManager.faceMaterials[biomeId];
        }

        if (throwVector.magnitude == 0)
        {
            Vector3 randomThrowVector = new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 0), Random.Range(-5f, 5f));
            throwVector = randomThrowVector;
        }
        Rigidbody diceRb = spawnedDice.GetComponent<Rigidbody>();
        Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        diceRb.AddForce(throwVector * throwForce, ForceMode.Impulse);
        diceRb.AddTorque(randomSpinVector.normalized * spinForce, ForceMode.Impulse);
        //diceManager.UpdateDiceData();
    }
}