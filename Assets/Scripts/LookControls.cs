using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LookControls : MonoBehaviour
{
    // Start is called before the first frame update
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;
    private Transform camTransform;
    private Camera cam;
    private float movement;
    private float xMovement;
    private float yMovement;
    private Vector3 camHolderRotation;
    private Vector3 camRotation;
    public Vector3 startingRotation;
    public Vector3 startingCamRotation;
    void Start()
    {
        //camTransform = GetComponentInChildren<Transform>();


        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        startingRotation = transform.position;
        startingCamRotation.y += startingRotation.y;
        //cam.transform.eulerAngles = startingCamRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward * 3);
    }

    // Update is called once per frame
    void Update()
    {
        movement = -Input.GetAxisRaw("Horizontal");
        xMovement = Input.GetAxisRaw("Mouse X") * horizontalSensitivity;
        yMovement = -Input.GetAxisRaw("Mouse Y") * verticalSensitivity;

        //camHolderRotation = new Vector3(Mathf.Clamp(camHolderRotation.x, -90, 90), Mathf.Clamp(camHolderRotation.y + movement * 0.15f, -112 - startingRotation.y, 27 - startingRotation.y), camHolderRotation.z);
        camRotation = new Vector3(Mathf.Clamp(camRotation.x + yMovement, -90, 90), camRotation.y + xMovement, camRotation.z);

        //cam.transform.eulerAngles = startingCamRotation + camRotation;
        //transform.eulerAngles = camHolderRotation + startingRotation;
    }
}
