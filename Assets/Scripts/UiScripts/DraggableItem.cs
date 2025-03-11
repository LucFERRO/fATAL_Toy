using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image image;
    CanvasGroup group;
    private DiceManagerV2 diceManager;
    public Transform parentAfterDrag;
    public string diceColor;

    private void Start()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        diceManager = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<DiceManagerV2>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.parent.GetComponent<Image>().color = transform.parent.GetComponent<DiceSlot>().baseColor;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        diceManager.currentHeldDiceColor = diceColor;
        group.alpha = 0.5f;
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        group.alpha = 1f;
        diceManager.currentHeldDiceColor = "";
        image.raycastTarget = true;
    }
}