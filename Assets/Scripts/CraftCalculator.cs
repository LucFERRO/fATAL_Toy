using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftCalculator : MonoBehaviour
{
    //[SerializeField] public string[][] rollResults;
    //public List colorList;
    public List<FaceComponent> diceRollResults;
    public FaceComponent[] diceResultsArray;
    public List<string> colorList;
    public Dictionary<string, int> resultDictionary = new();
    void Start()
    {
        //string[] vectors = new string[5] { "up", "up","right", "up", "right"};
        //string[] colors = new string[5] { "green", "red","green", "blue", "red"};
        //rollResults = new string[][] { vectors, colors };
        Debug.Log("Liste: "+diceRollResults[2].faceType);
        Debug.Log(diceRollResults[2].faceColor);
        Debug.Log(" "+diceResultsArray[4].faceType);
        Debug.Log(diceResultsArray[4].faceColor);

        for (int i = 0; i < diceResultsArray.Length; i++)
        {
            string possibleNewColor = diceResultsArray[i].faceColor;
            if (!resultDictionary.ContainsKey(possibleNewColor))
            {
                resultDictionary.Add(possibleNewColor, 1);
            }
            else
            {
                resultDictionary[possibleNewColor]++;
            }
        }

        //for (int i = 0; i < resultDictionary.Count; i++)
        //{
        //    Debug.Log();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
