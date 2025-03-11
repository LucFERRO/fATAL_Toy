using UnityEngine;

public class PawnAreaData : MonoBehaviour
{
    public bool[] typeArray;
    public bool[] lvlArray;
    // Hunter, Village, Hut
    // [0, 1, 2]
    void Start()
    {
        typeArray = new bool[] { false, false, false };
        lvlArray = new bool[] { false, false };
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("CraftArea"))
        {
            return;
        }
        SubArea subArea = other.gameObject.GetComponent<SubArea>();

        if (subArea.craftLvl == 0)
        {
            typeArray[subArea.craftType] = true;
        }

        else
        {
            lvlArray[subArea.craftLvl - 1] = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("CraftArea"))
        {
            return;
        }
        SubArea subArea = other.gameObject.GetComponent<SubArea>();

        if (subArea.craftLvl == 0)
        {
            typeArray[subArea.craftType] = false;
        }

        else
        {
            lvlArray[subArea.craftLvl - 1] = false;
        }
    }
}
