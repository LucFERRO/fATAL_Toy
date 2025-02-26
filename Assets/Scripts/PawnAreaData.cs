using UnityEngine;

public class PawnAreaData : MonoBehaviour
{
    public bool[] typeArray;
    public bool[] lvlArray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        typeArray = new bool[] {false, false, false};
        lvlArray = new bool[] {false, false, false};
    }
}
