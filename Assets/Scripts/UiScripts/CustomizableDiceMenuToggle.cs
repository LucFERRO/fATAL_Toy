using System;
using UnityEngine;

public class CustomizableDiceMenuToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameManager gameManager;
    public int uiDistance;
    private Vector3[] startingPositions;
    private bool isInventoryHidden;
    private bool IsInventoryHidden
    {
        get { return isInventoryHidden; }
        set
        {
            isInventoryHidden = value;
            HandleInventoryMenu(isInventoryHidden ? uiDistance : 0);
        }
    }


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
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsInventoryHidden = !IsInventoryHidden;
        }

    }
    private void MoveInventoryUi(GameObject go, Vector3 targetPos)
    {
        //go.transform.position = Vector3.Lerp(go.transform.position, targetPos, Time.deltaTime);
        go.transform.position = targetPos;
    }

    private void HandleInventoryMenu(int distanceToMove)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MoveInventoryUi(transform.GetChild(i).gameObject, startingPositions[i] + new Vector3(distanceToMove,0,0));
        }
    }
}
