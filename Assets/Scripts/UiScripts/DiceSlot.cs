using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;

public class DiceSlot : MonoBehaviour, IDropHandler
{
    public Color baseColor;
    private void Start()
    {
        baseColor = transform.GetComponent<Image>().color;
        GetColorFromChild();
    }
    private void OnTransformChildrenChanged()
    {
        GetColorFromChild();
    }

    private void GetColorFromChild()
    {
        if (transform.childCount != 0)
        {
            Color chosenColor;
            if (ColorUtility.TryParseHtmlString(transform.GetChild(0).GetComponent<DraggableItem>().diceColor, out chosenColor))
            {
                transform.GetComponent<Image>().color = chosenColor;
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Color chosenColor;
            if (ColorUtility.TryParseHtmlString(eventData.pointerDrag.GetComponent<DraggableItem>().diceColor, out chosenColor))
            {
                transform.GetComponent<Image>().color = chosenColor;
            }
            eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
        }
    }
}