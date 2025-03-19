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
        if (transform.childCount != 0)
        {
            transform.GetChild(0).SetParent(eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag);
        }
        eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
    }
}