using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    GameObject[] uiUnlockableTilesGO;
    DraggableItem[] uiUnlockableTilesItems;

    string[] tileTypes;

    [SerializeField]
    string test;

    void Start()
    {
        uiUnlockableTilesItems = new DraggableItem[uiUnlockableTilesGO.Length];
        gameManager = GetComponent<GameManager>();
        for (int i = 0; i < uiUnlockableTilesGO.Length; i++)
        {
            uiUnlockableTilesItems[i] = uiUnlockableTilesGO[i].transform.GetChild(0).GetComponent<DraggableItem>();
        }

        tileTypes = gameManager.tileTypes;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            HandleUnlockComboTile(test);
        }
    }

    public void HandleUnlockComboTile(string potentialNewCombo)
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (potentialNewCombo == "forest" || potentialNewCombo == "lake" || potentialNewCombo == "mountain" || potentialNewCombo == "plain")
            {
                return;
            }
            if (potentialNewCombo == tileTypes[i])
            {
                Debug.Log($"New combo tile {tileTypes[i]} unlocked, Ui N`{i} now available!");
                uiUnlockableTilesItems[i].isAvailable = true;
                uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();
            }
        }
    }
}
