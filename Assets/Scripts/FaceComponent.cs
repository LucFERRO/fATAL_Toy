using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceComponent : MonoBehaviour
{
    public string faceType;
    [HideInInspector]public string faceColor;
    private void Start()
    {
        faceColor = transform.parent.GetComponent<DiceData>().diceColor;
    }
}