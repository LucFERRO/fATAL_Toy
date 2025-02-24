using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterNavigation : MonoBehaviour
{
    public Transform[] destinations;
    public Transform currentDestination;
    public int destinationIndex;
    private NavMeshAgent hunterAgent;
    public float destinationRadius;
    // Start is called before the first frame update
    void Start()
    {
        hunterAgent = GetComponent<NavMeshAgent>();
        destinationIndex = 0;
        currentDestination = destinations[destinationIndex];
        hunterAgent.SetDestination(currentDestination.position);
    }

    // Update is called once per frame
    void Update()
    {
    // Check if the agent is close enough to the actual destination, and switch to the next if so. 
        if (hunterAgent.remainingDistance <= destinationRadius) {
            if (destinationIndex == destinations.Length - 1)
            {
                destinationIndex = 0;
            }
            else
            {
                destinationIndex += 1;
            }
            UpdateDestination();
        }
    }

    // Set the next destination for the agent.
    void UpdateDestination()
    {
        currentDestination = destinations[destinationIndex];
        hunterAgent.SetDestination(currentDestination.position);
        Debug.Log("agent destination: " + hunterAgent.destination);
        Debug.Log("agent position: " + transform.position);
        Debug.Log("remaining distance: " + hunterAgent.remainingDistance);
    }
}
