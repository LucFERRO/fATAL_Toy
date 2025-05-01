using System;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    [SerializeField] TileType test;
    string[] tileTypes;

    GameManager gameManager;
    [Header("References")]
    [SerializeField] GameObject biomeUiContainer;
    [SerializeField] DraggableItem[] uiUnlockableTilesItems;
    [SerializeField] GameObject diceFacesGO;
    [SerializeField] GameObject baseBiomesGO;
    [SerializeField] GameObject doubleBiomesGO;
    [SerializeField] GameObject comboBiomesGO;
    [SerializeField] GameObject[] uiUnlockableTilesGO;


    void Start()
    {
        gameManager = GetComponent<GameManager>();

        uiUnlockableTilesItems = new DraggableItem[uiUnlockableTilesGO.Length];
        for (int i = 0; i < uiUnlockableTilesGO.Length; i++)
        {
            uiUnlockableTilesItems[i] = uiUnlockableTilesGO[i].transform.GetChild(0).GetComponent<DraggableItem>();
        }

        tileTypes = Enum.GetNames(typeof(TileType));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleUnlockComboTile(test);
        }
    }

    public void HandleUnlockComboTile(TileType potentialNewCombo)
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if((int)potentialNewCombo < 10)
            {
                return;
            }
            if (potentialNewCombo.ToString() == tileTypes[i])
            {
                uiUnlockableTilesItems[i].isAvailable = true;
                uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();
            }
        }
    }
}
