using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    //public Color baseColor;
    private void Start()
    {
        //baseColor = transform.GetComponent<Image>().color;
        //GetColorFromChild();
    }
    //private void OnTransformChildrenChanged()
    //{
    //    GetColorFromChild();
    //}

    //private void GetColorFromChild()
    //{
    //    if (transform.childCount != 0)
    //    {
    //    }
    //}
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
        }
        else
        {
            //Item item = transform.GetChild(0).GetComponent<Item>();
            //Item draggedItem = eventData.pointerDrag.GetComponent<Item>();
            //if (draggedItem.itemId == item.itemId)
            //{
            //    if (draggedItem.currentStack + item.currentStack <= item.maxStack)
            //    {
            //        item.currentStack += draggedItem.currentStack;
            //        transform.GetChild(0).GetComponent<DraggableItem>().HandleStackableItems();
            //        Destroy(draggedItem.gameObject);
            //    } else
            //    {
            //        draggedItem.currentStack = (draggedItem.currentStack + item.currentStack)%item.maxStack;
            //        item.currentStack = item.maxStack;
            //        transform.GetChild(0).GetComponent<DraggableItem>().HandleStackableItems();
            //        eventData.pointerDrag.GetComponent<DraggableItem>().HandleStackableItems();
            //    }
            //}
        }
    }
}