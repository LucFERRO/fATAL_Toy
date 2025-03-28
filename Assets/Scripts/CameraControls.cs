using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;
    public float rotationSpeed;
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
        cam = GetComponentInChildren<Camera>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //startingRotation = transform.position;
        startingCamRotation.x += startingRotation.x;
        cam.transform.eulerAngles = startingCamRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement = -Input.GetAxisRaw("Horizontal");
        //xMovement = Input.GetAxisRaw("Mouse X") * horizontalSensitivity;
        //yMovement = -Input.GetAxisRaw("Mouse Y") * verticalSensitivity;

        camHolderRotation = new Vector3(camHolderRotation.x,camHolderRotation.y + movement * rotationSpeed, camHolderRotation.z);
        camRotation = new Vector3(camRotation.x, camRotation.y + movement * rotationSpeed, camRotation.z);

        cam.transform.eulerAngles = startingCamRotation + camRotation;
        transform.eulerAngles = camHolderRotation + startingRotation;
    }
}
