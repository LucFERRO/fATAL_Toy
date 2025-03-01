using UnityEngine;

public class SubArea : CraftingAreas
{
    public int craftLvl;

    //public void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("SAMARCHE");
    //    if (!other.gameObject.CompareTag("CraftingPawn"))
    //    {
    //        return;
    //    }
    //    PawnAreaData pawnAreaData = other.gameObject.GetComponent<PawnAreaData>();

    //    if (craftLvl == 0)
    //    { 
    //        pawnAreaData.typeArray[craftType] = true;
    //    }

    //    pawnAreaData.lvlArray[craftLvl] = true;
    //}
    //public void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("SAMARCHE_EXIT" + other.name);
    //    if (!other.gameObject.CompareTag("CraftingPawn"))
    //    {
    //        return;
    //    }
    //    PawnAreaData pawnAreaData = other.gameObject.GetComponent<PawnAreaData>();

    //    if (craftLvl == 0)
    //    {
    //        pawnAreaData.typeArray[craftType] = false;
    //    }

    //    pawnAreaData.lvlArray[craftLvl] = false;
    //}
}
