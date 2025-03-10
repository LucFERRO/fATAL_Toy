using UnityEngine;

public class CraftingAreas : MonoBehaviour
{
    private Collider craftCollider;
    public int craftType; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        craftCollider = GetComponent<Collider>();
    }
}
