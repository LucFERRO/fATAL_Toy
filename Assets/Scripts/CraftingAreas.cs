using UnityEngine;

public class CraftingAreas : MonoBehaviour
{
    private Collider craftCollider;
    public int craftType; 

    void Start()
    {
        craftCollider = GetComponent<Collider>();
    }
}
