using UnityEngine;

public class DiceDropArea : MonoBehaviour, IDiceDropArea
{
    public void OnItemDrop(BiomeTileItem item)
    {
        item.transform.position = transform.position;
        Debug.Log("dropped on dice");
    }
}