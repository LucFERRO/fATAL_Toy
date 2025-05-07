using UnityEngine;

public class BiomeTileItem : MonoBehaviour
{
    private Collider2D col;
    private Vector3 startDragPosition;
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        startDragPosition = transform.position;
        transform.position = GetMousePositionInWorldSpace();
    }
    private void OnMouseDrag()
    {
        transform.position = GetMousePositionInWorldSpace();
    }
    private void OnMouseUp()
    {
        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;
        if (hitCollider != null && hitCollider.TryGetComponent(out IDiceDropArea diceDropArea))
        {
            diceDropArea.OnItemDrop(this);
        } else
        {
            transform.position = startDragPosition;
        }
    }

    public Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        return position;
    }
}