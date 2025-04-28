using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private DraggableItem draggableItem;
    private Image inventoryImage;
    private Image biomeImage;
    private void Start()
    {
        draggableItem = transform.GetChild(0).GetComponent<DraggableItem>();
        inventoryImage = GetComponent<Image>();
        biomeImage = transform.GetChild(0).GetComponent<Image>();
        if (!draggableItem.isOnDice)
        {
            EnableInventorySlot();
        }
    }

    public void EnableInventorySlot()
    {
        inventoryImage.enabled = draggableItem.isAvailable;
        biomeImage.enabled = draggableItem.isAvailable;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount != 0)
        {
            Transform targetDiceFaceTransform = transform.GetChild(0);
            bool targetFaceIsOnDice = targetDiceFaceTransform.GetComponent<DraggableItem>().isOnDice;
            bool heldFaceIsOnDice = eventData.pointerDrag.GetComponent<DraggableItem>().isOnDice;

            if (targetFaceIsOnDice && !heldFaceIsOnDice)
            {
                Destroy(targetDiceFaceTransform.gameObject);
                GameObject createdDiceFace = Instantiate(eventData.pointerDrag, transform);
                DraggableItem createdDraggableItem = createdDiceFace.GetComponent<DraggableItem>();
                createdDraggableItem.isOnDice = true;
                createdDraggableItem.group.alpha = 1f;
                createdDraggableItem.image.raycastTarget = true;
                createdDraggableItem.parentAfterDrag = transform;
                return;
            }
            else if (!targetFaceIsOnDice && heldFaceIsOnDice)
            {
                Destroy(eventData.pointerDrag);
                return;
            }
            else
            {
                targetDiceFaceTransform.SetParent(eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag);
            }
        }
        eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
    }
}