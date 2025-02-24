using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceData : MonoBehaviour
{
    [HideInInspector] public int numberOfFaces = 6;
    [HideInInspector] public FaceComponent[] faceComponentArray;
    public bool isInUse;
    private DiceManager resultManager;
    public string diceColor;
    public int chosenFaceIndex;

    void Start()
    {
        resultManager = transform.parent.GetComponent<DiceManager>();
        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            Debug.Log(i);
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
        }
    }
    public void UpdateRollResult()
    {
        FaceComponent[] faceComponentArray = new FaceComponent[numberOfFaces];
        float[] vectorDotResultArray = new float[numberOfFaces];
        float closestVectorDot = 0;
        int closestIndex = 0;

        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = faceComponentArray[i];
            // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
            // Produit scalaire entre le vector.up de chaque face et Vector3.up
            vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);

            // Garde la face qui a son vecteur le plus vertical
            if (vectorDotResultArray[i] >= closestVectorDot)
            {
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }
        chosenFaceIndex = closestIndex;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            // Si clique pour ajouter un dé inactif alors que déjà le nombre max de dés sélectionnés
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