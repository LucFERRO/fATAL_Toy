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
    private DiceManagerV2 diceManager;
    public string diceColor;
    public string diceRarity;
    //useless?
    public int chosenFaceIndex;

    void Start()
    {
        diceManager = transform.parent.GetComponent<DiceManagerV2>();
        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
        }
        isInUse = true;
        InitiateVanillaDice();
    }

    private void InitiateVanillaDice()
    {
        Vector2[] vectors = diceManager.possibleDiceVectors;
        string[] colors = diceManager.possibleDiceColors;
        //string[] rarities = diceManager.possibleDiceRarities;

        string randomColor = colors[UnityEngine.Random.Range(0, colors.Length)];
        diceColor = randomColor;

        for (int i = 0; i < faceComponentArray.Length; i++)
        {
            FaceComponent face = faceComponentArray[i];
            Vector2 randomVector = vectors[UnityEngine.Random.Range(0, vectors.Length)];
            face.faceVector = randomVector.normalized;
            face.faceColor = randomColor;
        }
    }

    public FaceComponent GetRandomFace()
    {
        int randomInt = UnityEngine.Random.Range(0, numberOfFaces);
        return faceComponentArray[randomInt];
    }

    private void UpdateFaceColors()
    {
        for (int i = 0; i < faceComponentArray.Length; i++)
        {
            FaceComponent face = faceComponentArray[i];
            face.faceColor = diceColor;
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
        if (Input.GetMouseButtonDown(0))
        {
            // Si clique pour ajouter un dé inactif alors que déjà le nombre max de dés sélectionnés
            if (!isInUse && diceManager.numberOfDicesInUse >= diceManager.maxNumberOfDices)
            {
                string selectedAllDicesAlreadyMessage = $"Cannot use more than {diceManager.maxNumberOfDices} dices!";
                Debug.Log(selectedAllDicesAlreadyMessage);
                diceManager.uiManager.DisplayErrorMessage(selectedAllDicesAlreadyMessage);
                return;
            }

            isInUse = !isInUse;
            diceManager.NumberOfDicesInUse += isInUse ? 1 : -1;
        }

        if (Input.GetMouseButtonUp(0))
        {
            diceColor = diceManager.currentHeldDiceColor;
            UpdateFaceColors();
        }
    }
}