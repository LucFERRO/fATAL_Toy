using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceData : MonoBehaviour
{
    public int numberOfFaces;
    public bool isInUse;
    [HideInInspector] public GameObject[] facesArray;
    private DiceManager resultManager;

    void Start()
    {
        resultManager = transform.parent.GetComponent<DiceManager>();
        facesArray = new GameObject[numberOfFaces];
        for (int i = 0; i < transform.childCount; i++)
        {
            facesArray[i] = transform.GetChild(i).gameObject;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            // Si clique pour ajouter un d� inactif alors que d�j� le nombre max de d�s s�lectionn�s
            if (!isInUse && resultManager.numberOfDicesInUse >= resultManager.maxNumberOfDices)
            {
                Debug.Log($"Cannot use more than {resultManager.maxNumberOfDices} dices!");
                return;
            }

            isInUse = !isInUse;
            resultManager.NumberOfDicesInUse += isInUse ? 1 : -1;
        }
    }
}