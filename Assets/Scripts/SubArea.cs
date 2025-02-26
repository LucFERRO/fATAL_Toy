using UnityEngine;

public class SubArea : CraftingAreas
{
    public int craftLvl;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TEST");
        if (!other.CompareTag("CraftingPawn"))
        {
            return;
        }
        PawnAreaData pawnAreaData = other.GetComponent<PawnAreaData>();

        if (craftLvl == 0)
        { 
            pawnAreaData.typeArray[craftType] = true;
        }

        pawnAreaData.lvlArray[craftLvl] = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("CraftingPawn"))
        {
            return;
        }
        PawnAreaData pawnAreaData = other.GetComponent<PawnAreaData>();

        if (craftLvl == 0)
        {
            pawnAreaData.typeArray[craftType] = false;
        }

        pawnAreaData.lvlArray[craftLvl] = false;
    }
}
