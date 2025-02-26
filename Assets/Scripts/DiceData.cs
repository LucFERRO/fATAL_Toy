using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            faceComponentArray[i] = transform.GetChild(0).GetComponent<FaceComponent>();
        }
    }
    public int UpdateRollResult()
    {
        float[] vectorDotResultArray = new float[numberOfFaces];
        float closestVectorDot = 0;
        int closestIndex = 0;

        for (int i = 0; i < numberOfFaces; i++)
        {
            // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
            // Produit scalaire entre le vector.up de chaque face et Vector3.up
            vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);
            //Debug.Log(vectorDotResultArray[i]);
            // Garde la face qui a son vecteur le plus vertical
            if (vectorDotResultArray[i] >= closestVectorDot)
            {
                //Debug.Log("new index detected " + i);
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }
        //chosenFaceIndex = closestIndex;
        int chosenIndex = Array.IndexOf(vectorDotResultArray, vectorDotResultArray.Max());
        Debug.Log($"chosenIndex of {transform.gameObject.name} is {chosenIndex}");
        return chosenIndex;
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