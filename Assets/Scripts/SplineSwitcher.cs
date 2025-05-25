using UnityEngine;
using UnityEngine.Splines;
using Unity.Cinemachine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class SplineSwitcher : MonoBehaviour
{
    public SplineContainer[] splinesArray;
    public CinemachineCamera[] virtualCameras;
    public CinemachineCamera domeCamera;

    private CinemachineSplineDolly[] cinemachineSplineDollies;
    public bool isIdle = false;

    public Animator canvasAnimator;
    public Animator spacebarAnimator;
    public GameManager gameManager;
    public Inventory inventory;

    private int currentIndex = 0;
    float time = 0f;

    public int maxTimerToShowSpacebar;
    private float currentTimerToSpace;

    void Start()
    {
        domeCamera.gameObject.SetActive(true);
        cinemachineSplineDollies = new CinemachineSplineDolly[splinesArray.Length];
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            cinemachineSplineDollies[i] = virtualCameras[i].GetComponent<CinemachineSplineDolly>();
        }
        currentTimerToSpace = maxTimerToShowSpacebar;
    }

    private void Update()
    {
        CheckCameraInput();
        CheckInput();
        if (currentTimerToSpace > 0)
        {
            currentTimerToSpace -= Time.deltaTime;
            spacebarAnimator.SetBool("IsBlinking", false);
        } else
        {
            spacebarAnimator.SetBool("IsBlinking", true);
        }

        if (Input.GetKey(KeyCode.Space) && !isIdle)
        {
            time += Time.deltaTime;
            if (time >= 1f)
            {
                Debug.Log("Entering Idle");
                SwitchToIdle();
                time = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.Space) && isIdle)
        {
            time += Time.deltaTime;
            if (time >= 1f)
            {
                Debug.Log("Exiting Idle");
                ExitIdle();
                time = 0f;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            time = 0f;
        }

        // Check for input to switch splines
        if (Input.GetKeyDown(KeyCode.P) && isIdle)
        {
            SwitchCamera(currentIndex + 1);
        }
    }

    private void CheckInput()
    {
        if (!isIdle)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.DownArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ExitIdle();
        }
    }

    private void CheckCameraInput()
    {
        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0) || isIdle)
        {
            currentTimerToSpace = maxTimerToShowSpacebar;
        }
    }

    private void ExitIdle()
    {
        isIdle = false;
        virtualCameras[currentIndex].gameObject.SetActive(false);
        splinesArray[currentIndex].gameObject.SetActive(false);
        domeCamera.gameObject.SetActive(true);
        canvasAnimator.SetTrigger("ToggleTrigger");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inventory.interfaceEventInstance.setParameterByName("IsInInventory", 1);
        inventory.interfaceEventInstance.start();
    }
    private void SwitchToIdle()
    {
        isIdle = true;
        gameManager.isPreviewing = false;
        spacebarAnimator.SetBool("IsBlinking", false);
        canvasAnimator.SetTrigger("ToggleTrigger");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        domeCamera.gameObject.SetActive(false);
        SetActiveSplineAndCamera(currentIndex);
        // son ouvre inventory
        inventory.interfaceEventInstance.setParameterByName("IsInInventory", 0);
        inventory.interfaceEventInstance.start();
    }

    private void SetActiveSplineAndCamera(int index)
    {
        virtualCameras[index].gameObject.SetActive(true);
        splinesArray[index].gameObject.SetActive(true);
    }

    private void SwitchCamera(int index)
    {
        virtualCameras[currentIndex].gameObject.SetActive(false);
        splinesArray[currentIndex].gameObject.SetActive(false);

        currentIndex = (int) Mathf.Repeat(index, splinesArray.Length);

        SetActiveSplineAndCamera(currentIndex);
    }
}
