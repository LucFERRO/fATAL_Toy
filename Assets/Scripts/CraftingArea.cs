using UnityEngine;

public class CraftingArea : MonoBehaviour
{
    private Collider craftCollider;
    public int craftId;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        craftCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
