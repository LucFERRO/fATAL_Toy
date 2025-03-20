using UnityEngine;

public class CustomizableDiceMenuToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameManager gameManager;
    private Vector3[] startingPositions;
    void Start()
    {
        startingPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            startingPositions[i] = transform.GetChild(i).transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
