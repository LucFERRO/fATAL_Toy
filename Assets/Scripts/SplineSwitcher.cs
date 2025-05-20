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

    private int currentIndex = 0;
    float time = 0f;

    void Start()
    {
        domeCamera.gameObject.SetActive(true);
        cinemachineSplineDollies = new CinemachineSplineDolly[splinesArray.Length];
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            cinemachineSplineDollies[i] = virtualCameras[i].GetComponent<CinemachineSplineDolly>();
        }
    }

    private void Update()
    {
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

    private void SwitchToIdle()
    {
        isIdle = true;
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
