using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    private void Start()
    {

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