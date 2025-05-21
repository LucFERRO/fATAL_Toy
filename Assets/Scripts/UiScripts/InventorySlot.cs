using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    //private DraggableItem draggableItem;
    //private Image inventoryImage;
    //private Image inventoryImageLocked;
    //private Image biomeImage;
    //private Image biomeImageLocked;
    //private GameObject biomeGo;
    //private GameObject hiddenBiomeGo;

    //public bool isCombo;

    public bool isDicePanelSlot = false; // Determines if this slot belongs to the dice equipment panel
    public Sprite unlockedBiome;
    public DraggableItem currentlyDraggedItem;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMouseOver()
    {
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
            foreach (Transform child in transform.parent) // Assuming all dice slots share the same parent
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
    //private void Start()
    //{
    //    draggableItem = transform.GetChild(0).GetComponent<DraggableItem>();
    //    inventoryImage = GetComponent<Image>();
    //    biomeImage = transform.GetChild(0).GetComponent<Image>();
    //    biomeGo = transform.GetChild(0).gameObject;
    //    if (!draggableItem.isOnDice)
    //    {
    //        if (isCombo)
    //        {
    //            hiddenBiomeGo = transform.GetChild(1).gameObject;
    //            //    biomeImageLocked = transform.GetChild(1).GetComponent<Image>();
    //        }
    //        EnableInventorySlot();
    //    }
    //}

    //public void EnableInventorySlot()
    //{
        //    //inventoryImage.enabled = draggableItem.isAvailable;
        //    //biomeImage.enabled = draggableItem.isAvailable;
        //    //if (!draggableItem.isOnDice && isCombo)
        //    //{
        //    //    //biomeImageLocked.enabled = draggableItem.isAvailable;
        //    //    //biomeImage.enabled = !draggableItem.isAvailable;
        //    //    biomeGo.SetActive(draggableItem.isAvailable);
        //    //    hiddenBiomeGo.SetActive(!draggableItem.isAvailable);
        //    //}
    //}

    public void UnlockChildBiomeIcon()
    {
        Transform childTransform = transform.GetChild(0);
        childTransform.GetComponent<Image>().sprite = unlockedBiome;
        childTransform.GetComponent<DraggableItem>().isDraggable = true;
    }

    //public void OnDrop(PointerEventData eventData)
    //{
    //    if (transform.childCount != 0)
    //    {
    //        Transform targetDiceFaceTransform = transform.GetChild(0);
    //        bool targetFaceIsOnDice = targetDiceFaceTransform.GetComponent<DraggableItem>().isOnDice;
    //        bool heldFaceIsOnDice = eventData.pointerDrag.GetComponent<DraggableItem>().isOnDice;

    //        if (targetFaceIsOnDice && !heldFaceIsOnDice)
    //        {
    //            Destroy(targetDiceFaceTransform.gameObject);
    //            GameObject createdDiceFace = Instantiate(eventData.pointerDrag, transform);
    //            DraggableItem createdDraggableItem = createdDiceFace.GetComponent<DraggableItem>();
    //            createdDraggableItem.isOnDice = true;
    //            createdDraggableItem.group.alpha = 1f;
    //            createdDraggableItem.image.raycastTarget = true;
    //            createdDraggableItem.parentAfterDrag = transform;
    //            return;
    //        }
    //        else if (!targetFaceIsOnDice && heldFaceIsOnDice)
    //        {
    //            //Destroy(eventData.pointerDrag);
    //            return;
    //        }
    //        else
    //        {
    //            targetDiceFaceTransform.SetParent(eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag);
    //        }
    //    }
    //    eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
    //}
}