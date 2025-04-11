using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalDiceSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject gameManagerGO;
    public GameObject diceToSpawn;
    public Slider diceSlider;

    [Header("Dice Properties")]
    public float diceSize = 100f;
    public int minSize = 1;
    public int maxSize = 150;

    [Header("Movement Settings")]
    public int movementSpeed;
    public int spinForce;
    public int throwForce;
    public float distance;
    public float positionSwitchTimer;

    private GameManager gameManager;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        Initialize();
    }

    //void Update()
    //{
    //    HandlePositionSwitching();
    //    HandleDiceSpawn();
    //}

    private void Initialize()
    {
        gameManager = gameManagerGO.GetComponent<GameManager>();
        startingPosition = transform.position;
        timer = positionSwitchTimer;

        diceSlider.wholeNumbers = true;
        diceSlider.minValue = minSize;
        diceSlider.maxValue = maxSize;
        diceSlider.value = diceSize;
    }

    //private void HandlePositionSwitching()
    //{
    //    timer -= Time.deltaTime;
    //    if (timer <= 0)
    //    {
    //        timer = positionSwitchTimer;
    //        targetPosition = new Vector3(Random.Range(-distance, distance), 0, Random.Range(-distance, distance)) + startingPosition;
    //    }

    //    transform.position = Vector3.Slerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    //}

    //private void HandleDiceSpawn()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SpawnDice(Vector3.zero, transform);
    //    }
    //}

    //OLD
    //public void SpawnDice(Vector3 throwVector, Transform originTransform)
    //{
    //    if (gameManagerGO.transform.childCount != 0)
    //    {
    //        return;
    //    }
    //    GameObject spawnedDice = Instantiate(diceToSpawn, originTransform.position + originTransform.forward * 3, originTransform.rotation);
    //    spawnedDice.transform.parent = gameManagerGO.transform;
    //    spawnedDice.transform.localScale = Vector3.one * diceSize * 0.01f;

    //    AssignFaceMaterials(spawnedDice);
    //    ApplyRandomThrowForce(spawnedDice, throwVector);
    //}    


    public void SpawnDice(Vector3 throwVector, Transform originTransform)
    {
        if (gameManagerGO.transform.childCount != 0)
        {
            return;
        }
        GameObject spawnedDice = Instantiate(diceToSpawn, originTransform.position, originTransform.rotation);
        spawnedDice.transform.parent = gameManagerGO.transform;
        spawnedDice.transform.localScale = Vector3.one * diceSize * 0.01f;
        spawnedDice.GetComponent<Rigidbody>().AddForce(throwVector, ForceMode.Impulse);
        AssignFaceMaterials(spawnedDice);
        ApplyRandomTorque(spawnedDice);
    }

    private void AssignFaceMaterials(GameObject spawnedDice)
    {
        for (int i = 0; i < gameManager.diceFaces.Length; i++)
        {
            Transform face = spawnedDice.transform.GetChild(i);
            int biomeId = gameManager.diceFaces[i].transform.GetChild(0).GetComponent<DraggableItem>().biomeId;

            FaceComponent faceComponent = face.GetComponent<FaceComponent>();
            MeshRenderer meshRenderer = face.GetComponent<MeshRenderer>();

            faceComponent.faceType = gameManager.tileTypes[biomeId];
            meshRenderer.material = gameManager.faceMaterials[biomeId];
            //test
        }
    }

    private void ApplyRandomThrowForce(GameObject spawnedDice, Vector3 throwVector)
    {
        if (throwVector.magnitude == 0)
        {
            throwVector = new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 0), Random.Range(-5f, 5f));
        }

        Rigidbody diceRb = spawnedDice.GetComponent<Rigidbody>();
        Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        diceRb.AddForce(throwVector * throwForce, ForceMode.Impulse);
        diceRb.AddTorque(randomSpinVector.normalized * spinForce, ForceMode.Impulse);
    }    
    private void ApplyRandomTorque(GameObject spawnedDice)
    {
        Rigidbody diceRb = spawnedDice.GetComponent<Rigidbody>();
        Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        diceRb.AddTorque(randomSpinVector.normalized * spinForce, ForceMode.Impulse);
    }

    public void ChangeDiceScale()
    {
        diceSize = diceSlider.value;
    }
}
