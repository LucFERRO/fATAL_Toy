using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool isDicePanelSlot;
    public Sprite unlockedBiome;
    public DraggableItem currentlyDraggedItem;
    private static GameManager gameManager;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (gameManager == null)
        {
            GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");
            gameManager = gameManagerGO.GetComponent<GameManager>();
        }
    }

    public void UnlockChildBiomeIcon()
    {
        Transform childTransform = transform.GetChild(0);
        childTransform.GetComponent<Image>().sprite = unlockedBiome;
        childTransform.GetComponent<DraggableItem>().isDraggable = true;
    }

    public void OnMouseOver()
    {
        Debug.Log(transform.name);
        if (!isDicePanelSlot)
        {
            return;
        }
        if (currentlyDraggedItem == null)
        {
            return;
        }
        animator.SetBool("isMouseDraggingOver", true);
    }
    public void OnMouseExit()
    {
        Debug.Log("exited " + transform.name);
        if (!isDicePanelSlot)
        {
            return;
        }
        if (currentlyDraggedItem == null)
        {
            return;
        }
        animator.SetBool("isMouseDraggingOver", false);
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
            foreach (Transform child in transform.parent)
            {
                DraggableItem childItem = child.GetComponentInChildren<DraggableItem>();
                if (childItem != null && childItem.biomeId == draggedItem.biomeId)
                {
                    sameBiomeCount++;
                }
            }

            // If there are already 2 or more items with the same biomeId, reject the drop
            if (sameBiomeCount >= 2)
            {
                //Debug.Log("Cannot add more items with the same biomeId to the dice panel.");
                return;
            }

            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
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
        }
        else
        {
            // If this is an inventory slot, return the item to its original slot
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
        draggedItem = null;
    }
}