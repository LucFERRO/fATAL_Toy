using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameManager;
    public GameObject diceToSpawn;
    public DiceManager diceManager;
    public int movementSpeed;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    public float positionSwitchTimer;
    private float timer;
    void Start()
    {
        startingPosition = transform.position;
        timer = positionSwitchTimer;
        // x 1.5 - 11.5
        // z -4 - 2
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = positionSwitchTimer;
            targetPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-3f, 3f)) + startingPosition;
        } 
        transform.position = Vector3.Slerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    }

    public void SpawnDice()
    {
        GameObject spawnedDice = Instantiate(diceToSpawn, gameManager.transform);
        spawnedDice.transform.position = transform.position;
        Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        spawnedDice.GetComponent<Rigidbody>().AddTorque(randomSpinVector.normalized * 5f, ForceMode.Impulse);
        diceManager.UpdateDiceData();
    }
}
