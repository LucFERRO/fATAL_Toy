using UnityEngine;

public class DropdownMenu : MonoBehaviour
{
    public DiceManagerV2 diceManager;

    private void Start()
    {
        diceManager = GetComponent<DiceManagerV2>();
    }
    public virtual void HandleDropdownMenu(int val)
    {
        Debug.Log("Base dropdown menu value: " + val);
    }

}
