using UnityEngine;

public class ToggleIgnore : MonoBehaviour
{
    private bool toggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle) { 
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        } else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            toggle = !toggle;
        }
    }
}
