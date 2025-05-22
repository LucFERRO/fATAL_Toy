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
    public bool[] pendingUnlockedStatus;
    GameManager gameManager;
    [Header("References")]
    Animator[] tileComboAnimators;
    [SerializeField] GameObject tileComboTitleGO;
    DraggableItem[] uiUnlockableTilesItems;
    [SerializeField] GameObject[] uiUnlockableTilesGO;
    [SerializeField] Image[] lockIcons;
    [SerializeField] Sprite lockedSprite;
    [SerializeField] Sprite unusedSprite;
    [SerializeField] Sprite usedSprite;

    FMOD.Studio.EventInstance newLockEventInstance;

    void Start()
    {
        InitializeUnlockManager();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            UnlockEveryComboTile();
        }
    }

    private void InitializeUnlockManager()
    {
        gameManager = GetComponent<GameManager>();
        uiUnlockableTilesItems = new DraggableItem[uiUnlockableTilesGO.Length];
        tileComboAnimators = new Animator[uiUnlockableTilesGO.Length - 4];
        pendingUnlockedStatus = new bool[uiUnlockableTilesGO.Length - 4];
        for (int i = 0; i < uiUnlockableTilesGO.Length; i++)
        {
            Transform inventorySlotChildBiome = uiUnlockableTilesGO[i].transform.GetChild(0);
            uiUnlockableTilesItems[i] = inventorySlotChildBiome.GetComponent<DraggableItem>();
            if (i > 3)
            {
                tileComboAnimators[i - 4] = uiUnlockableTilesGO[i].transform.GetComponent<Animator>();
            }
        }

        tileTypes = Enum.GetNames(typeof(TileType));

        unlockStatus = new Dictionary<TileType, bool>();
        foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
        {
            unlockStatus[tileType] = (int)tileType < 10;
        }

        newLockEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UnlockLock");
    }

    private void UnlockEveryComboTile()
    {
        for (int i = 0; i < uiUnlockableTilesGO.Length; i++)
        {
            uiUnlockableTilesItems[i].isAvailable = true;
            if (i > 3)
            {
                unlockStatus[unlockStatus.Keys.ElementAt(i)] = true;
            }
            //uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();
        }

        int maxLockedTiles = unlockStatus.Values.Count(value => value);
        PendingUnlockStatus();
        if (GetComponent<UiManager>().isInventoryOpen)
        {
            ResolveUnlockStatus();
        }
        //Do not count the base tiles
        int newMaxLockedTiles = (maxLockedTiles - 4) / 2;
        gameManager.maxLockedTiles = newMaxLockedTiles;
        HandleLockIconUnlocks(newMaxLockedTiles);
    }

    public void PendingUnlockStatus()
    {
        for (int i = 0; i < pendingUnlockedStatus.Length; i++)
        {
            if (!pendingUnlockedStatus[i] && unlockStatus[unlockStatus.Keys.ToArray()[i + 4]])
            {
                pendingUnlockedStatus[i] = true;
            }
        }
    }
    public void ResolveUnlockStatus()
    {
        for (int i = 0; i < pendingUnlockedStatus.Length; i++)
        {
            if (pendingUnlockedStatus[i] && unlockStatus[unlockStatus.Keys.ToArray()[i + 4]])
            {
                tileComboAnimators[i].SetTrigger("UnlockTile");
            }
        }
    }

    public void HandleLockIconUnlocks(int maxLocks)
    {
        int currentLocks = NeighbourTileProcessor.currentLockedTiles;
        for (int i = 0; i < lockIcons.Length; i++)
        {
            if (i < currentLocks)
            {
                lockIcons[i].sprite = usedSprite;
            }
            else if (i < maxLocks)
            {
                lockIcons[i].sprite = unusedSprite;
            }
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
                //uiUnlockableTilesGO[i].GetComponent<InventorySlot>().EnableInventorySlot();

                if (!unlockStatus[potentialNewCombo])
                {
                    unlockStatus[potentialNewCombo] = true;
                    int maxLockedTiles = unlockStatus.Values.Count(value => value);
                    unlockStatus[unlockStatus.Keys.ElementAt(i)] = true;
                    pendingUnlockedStatus[i - 4] = true;
                    int newMaxLockedTiles = (maxLockedTiles - 4) / 2;
                    Debug.Log(newMaxLockedTiles);
                    if (maxLockedTiles %2 == 0 && maxLockedTiles > 4)
                    {
                        newLockEventInstance.setParameterByName("UnlockLockedLevel", newMaxLockedTiles);
                        newLockEventInstance.start();
                    }
                    gameManager.maxLockedTiles = newMaxLockedTiles;
                    HandleLockIconUnlocks(newMaxLockedTiles);
                    if (GetComponent<UiManager>().isInventoryOpen)
                    {
                        ResolveUnlockStatus();
                    }
                    return true;
                }
            }
        }
        return false;
    }
}