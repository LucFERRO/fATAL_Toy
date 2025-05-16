using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public CanvasGroup group;
    public Transform parentAfterDrag;
    public int biomeId;
    public bool isOnDice;
    public bool isAvailable;

    private void Start()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        if (isOnDice || !isAvailable)
        {
            return;
            //group.alpha = .5f;
        }
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isOnDice || !isAvailable)
        {
            return;
            //group.alpha = .5f;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (isOnDice)
        //{
        //    return;
        //    //group.alpha = .5f;
        //}
        if (isOnDice || !isAvailable)
        {
            return;
            //group.alpha = .5f;
        }
        transform.SetParent(parentAfterDrag);
        image.rectTransform.localScale = Vector3.one;
        group.alpha = 1f;
        image.raycastTarget = true;
    }
}