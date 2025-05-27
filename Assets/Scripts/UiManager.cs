using System.Collections;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Data")]
    public bool isPaused;
    public bool isLoadingMenu;
    public bool isConfirmReaload;
    public bool isInventoryOpen;
    public bool IsInventoryOpen
    {
        get { return isInventoryOpen; }
        set
        {
            isInventoryOpen = value;
            if (isInventoryOpen)
            {
                unlockManager.ResolveUnlockStatus();
            }
            inventoryAnimator.SetTrigger("ToggleTrigger");
        }
    }


    [Header("References")]
    public ProjectileThrow projectileThrow;
    public GameObject pauseMenu;
    public GameObject confirmReload;
    public Animator inventoryAnimator;
    private UnlockManager unlockManager;
    public GameObject inventoryGO;
    public GameObject biomesUiGO;
    public TileSplitManager tileSplitManager;

    private void Start()
    {
        unlockManager = GetComponent<UnlockManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            // SON BLURR 
        }
        if (isPaused)
        {
            Cursor.visible = true;
        }

        HandlePause();
    }
    public void HandlePause()
    {
        if (isPaused)
        {
            StartCoroutine(StopTimeCoroutine());
            //Time.timeScale = 0f;
        }
        else
        {
            //StartCoroutine(ResumeTimeCoroutine());
            Time.timeScale = 1f;
        }
    }

    public void FlatResumeTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    private IEnumerator ResumeTimeCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }    
    private IEnumerator StopTimeCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            projectileThrow.isOverUI = false;
        }

        isPaused = !isPaused;

        Cursor.lockState = CursorLockMode.None;
        confirmReload.SetActive(false);
        pauseMenu.SetActive(isPaused);
    }

    public void ToggleInventoryUI()
    {
        IsInventoryOpen = !IsInventoryOpen;
    }
}
