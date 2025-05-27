using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isDicePanelSlot;
    public Sprite unlockedBiome;
    public DraggableItem currentlyDraggedItem;
    private static GameManager gameManager;
    private static TileSplitManager tileSplitManager;
    Animator animator;
    FMOD.Studio.EventInstance interfaceEventInstance;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (gameManager == null)
        {
            GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");
            gameManager = gameManagerGO.GetComponent<GameManager>();

        }        
        if (tileSplitManager == null)
        {
            GameObject tileSplitManagerGO = GameObject.FindGameObjectWithTag("HexMap");
            tileSplitManager = tileSplitManagerGO.GetComponent<TileSplitManager>();

        }
        if (isDicePanelSlot)
        {
            interfaceEventInstance = transform.parent.parent.GetComponent<Inventory>().interfaceEventInstance;
        }
        else
        {
            interfaceEventInstance = transform.parent.parent.parent.parent.GetComponent<Inventory>().interfaceEventInstance;
        }
        //interfaceEventInstance.setParameterByName("IsObjectivePanel", 1);
    }

    public void UnlockChildBiomeIcon()
    {
        Transform childTransform = transform.GetChild(0);
        childTransform.GetComponent<Image>().sprite = unlockedBiome;
        childTransform.GetComponent<DraggableItem>().isDraggable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (!isDicePanelSlot)
        {
            if (transform.GetChild(0).GetComponent<DraggableItem>().isAvailable && draggedItem == null)
            {
                interfaceEventInstance.start();
                interfaceEventInstance.setParameterByName("UIState", 2);
                interfaceEventInstance.setParameterByName("IsInInventory", 0);
                interfaceEventInstance.getParameterByName("UIState", out float value);
                interfaceEventInstance.getParameterByName("IsInInventory", out float value2);
            }
            return;
        }

        if (draggedItem == null) return;

        currentlyDraggedItem = draggedItem;
        animator.SetBool("IsMouseDraggingOver", true);

        interfaceEventInstance.setParameterByName("UIState", 2);
        interfaceEventInstance.setParameterByName("IsInInventory", 1);
        interfaceEventInstance.start();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDicePanelSlot) return;

        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggedItem == null) return;

        currentlyDraggedItem = null;
        animator.SetBool("IsMouseDraggingOver", false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        currentlyDraggedItem = draggedItem;

        if (draggedItem == null) return;
        if (!draggedItem.isDraggable) return;

        if (isDicePanelSlot)
        {
            int sameBiomeCount = 0;
            List<Animator> sameTileOnDiceAnimators = new List<Animator>();
            foreach (Transform child in transform.parent)
            {
                DraggableItem childItem = child.GetComponentInChildren<DraggableItem>();
                if (childItem != null && childItem.biomeId == draggedItem.biomeId)
                {
                    sameBiomeCount++;
                    sameTileOnDiceAnimators.Add(child.GetComponent<Animator>());
                }
            }

            // If there are already 2 or more items with the same biomeId, reject the drop
            if (sameBiomeCount >= 2)
            {
                Debug.Log("Cannot add more items with the same biomeId to the dice panel.");
                foreach (Animator diceSlotAnimator in sameTileOnDiceAnimators)
                {
                    animator.SetBool("IsMouseDraggingOver", false);
                    diceSlotAnimator.SetTrigger("NoMore");
                }
                interfaceEventInstance.setParameterByName("UIState", 5);
                interfaceEventInstance.start();
                return;
            }

            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

            if (!gameManager.diceWasChanged)
            {
                Debug.Log("DICE WAS CHANGED");
                gameManager.diceWasChanged = true;
                tileSplitManager.UpdateObjectives();
            }

            // If this is a dice panel slot, clone the dragged item
            GameObject clonedItem = Instantiate(draggedItem.gameObject, transform);
            DraggableItem clonedDraggableItem = clonedItem.GetComponent<DraggableItem>();
            clonedDraggableItem.isDraggable = false; // Make the cloned item non-draggable
            clonedDraggableItem.childBackground.SetActive(false);
            CanvasGroup clonedGroup = clonedItem.GetComponent<CanvasGroup>();
            if (clonedGroup != null)
            {
                clonedGroup.alpha = 1f;
            }
            interfaceEventInstance.setParameterByName("UIState", 4);
            interfaceEventInstance.start();
        }
        else
        {
            // If this is an inventory slot, return the item to its original slot
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
        animator.SetBool("IsMouseDraggingOver", false);
        draggedItem = null;
    }
}