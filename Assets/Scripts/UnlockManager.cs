using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockManager : MonoBehaviour
{
    [SerializeField] TileType test;
    string[] tileTypes;
    private Dictionary<TileType, bool> unlockStatus;
    GameManager gameManager;
    [Header("References")]
    [SerializeField] GameObject tileComboTitleGO;
    DraggableItem[] uiUnlockableTilesItems;
    [SerializeField] GameObject[] uiUnlockableTilesGO;
    [SerializeField] Image[] lockIcons;
    [SerializeField] Sprite lockedSprite;
    [SerializeField] Sprite unusedSprite;
    [SerializeField] Sprite usedSprite;


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
            unlockStatus[tileType] = (int)tileType < 10;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < uiUnlockableTilesGO.Length; i++)
            {
                uiUnlockableTilesItems[i].isAvailable = true;
                if (i > 3)
                {
                    unlockStatus[unlockStatus.Keys.ElementAt(i)] = true;
                }
                uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();
            }

            int maxLockedTiles = unlockStatus.Values.Count(value => value);

            //Do not count the base tiles
            int newMaxLockedTiles = (maxLockedTiles - 4) / 2;
            gameManager.maxLockedTiles = newMaxLockedTiles;
            HandleLockIconUnlocks(newMaxLockedTiles);
        }
    }

    private void HandleLockIconUnlocks(int maxLocks)
    {
        for (int i = 0; i < lockIcons.Length; i++)
        {
            // Update the sprite of the first `maxLocks` elements to `unusedSprite`
            if (i < maxLocks)
            {
                lockIcons[i].sprite = unusedSprite;
            }
            // Update the remaining elements to `lockedSprite`
            else
            {
                lockIcons[i].sprite = lockedSprite;
            }
        }
    }

    public bool HandleUnlockComboTile(TileType potentialNewCombo)
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if ((int)potentialNewCombo < 10)
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
                    int maxLockedTiles = unlockStatus.Values.Count(value => value);
                    int newMaxLockedTiles = (maxLockedTiles - 4) / 2;
                    gameManager.maxLockedTiles = newMaxLockedTiles;
                    HandleLockIconUnlocks(newMaxLockedTiles);
                    return true;
                }
            }
        }
        return false;
    }
}