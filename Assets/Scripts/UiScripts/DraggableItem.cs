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
    public string diceRarity;
    public Vector2[] diceVectorArray;

    private void Start()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        diceManager = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<DiceManagerV2>();
        for (int i = 0; i < diceVectorArray.Length; i++)
        {
            diceVectorArray[i] = diceVectorArray[i].normalized;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.parent.GetComponent<Image>().color = transform.parent.GetComponent<DiceSlot>().baseColor;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        diceManager.currentlyHeldColor = diceColor;
        diceManager.currentlyHeldRarity = diceRarity;
        diceManager.currentlyHeldVectors = diceVectorArray;
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
        diceManager.currentlyHeldColor = "";
        diceManager.currentlyHeldVectors = new Vector2[6];
        diceManager.currentlyHeldRarity = "";
        image.raycastTarget = true;
    }
}