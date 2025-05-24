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
    public GameObject pauseMenu;
    public GameObject confirmReload;
    public Animator inventoryAnimator;
    private UnlockManager unlockManager;
    public GameObject inventoryGO;
    public GameObject biomesUiGO;

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

        HandlePause();
    }
    public void HandlePause()
    {
        if (isPaused)
        {
            StartCoroutine(StopTimeCoroutine());
        }
        else
        {
            //StartCoroutine(ResumeTimeCoroutine());
            Time.timeScale = 1f;
        }
    }

    private IEnumerator ResumeTimeCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;
    }    
    private IEnumerator StopTimeCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
    }

    public void ToggleInventoryUI()
    {
        IsInventoryOpen = !IsInventoryOpen;
    }

}
