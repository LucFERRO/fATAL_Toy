using UnityEngine;

public class ColorDropDown : DropdownMenu
{
    public override void HandleDropdownMenu(int val)
    {
        Debug.Log("Color dropdown menu value: " + diceManager.possibleDiceColors[val]);
    }
}
