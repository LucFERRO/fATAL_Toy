using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    [SerializeField] TileType test;
    string[] tileTypes;
    private Dictionary<TileType, bool> unlockStatus;

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

        unlockStatus = new Dictionary<TileType, bool>();
        foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
        {
            unlockStatus[tileType] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleUnlockComboTile(test);
        }        
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (var item in unlockStatus)
            {
                Debug.Log(item.Key +" "+ item.Value);
            }
        }
    }

    public bool HandleUnlockComboTile(TileType potentialNewCombo)
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if((int)potentialNewCombo < 10)
            {
                return false;
            }
            if (potentialNewCombo.ToString() == tileTypes[i])
            {
                uiUnlockableTilesItems[i].isAvailable = true;
                uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();

                if (!unlockStatus[potentialNewCombo])
                {
                    unlockStatus[potentialNewCombo] = true;
                    Debug.Log($"TileType {potentialNewCombo} {i} unlocked.");
                    return true;
                }
            }
        }
        return false;
    }
}
