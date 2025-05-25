using UnityEngine;
using UnityEngine.Splines;
using Unity.Cinemachine;
using static UnityEngine.Rendering.DebugUI;

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
                virtualCameras[currentIndex].gameObject.SetActive(false);
                splinesArray[currentIndex].gameObject.SetActive(false);
                domeCamera.gameObject.SetActive(true);
                isIdle = false;
                canvasAnimator.SetTrigger("ToggleTrigger");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
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

    private void CheckCameraInput()
    {
        if (isIdle)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || 
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q) || 
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentTimerToSpace = maxTimerToShowSpacebar;
        }
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
