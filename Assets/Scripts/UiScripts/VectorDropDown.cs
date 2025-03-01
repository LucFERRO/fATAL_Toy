using UnityEngine;

public class VectorDropDown : DropdownMenu
{
    public override void HandleDropdownMenu(int val)
    {
        Debug.Log("Vector dropdown menu value: " + diceManager.possibleDiceVectors[val]);
    }
}
