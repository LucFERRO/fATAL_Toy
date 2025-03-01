using UnityEngine;

public class RarityDropDown : DropdownMenu
{
    public override void HandleDropdownMenu(int val)
    {
        Debug.Log("Rarity dropdown menu value: " + diceManager.possibleDiceRarities[val]);
    }
}
