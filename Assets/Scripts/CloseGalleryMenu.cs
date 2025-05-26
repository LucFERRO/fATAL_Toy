using UnityEngine;

public class CloseGalleryMenu : MonoBehaviour
{
    Animator galleryAnimator;
    public Animator titleAnimator;
    private LoadScript loadScript;

    private void Start()
    {
        galleryAnimator = transform.parent.parent.GetComponent<Animator>();
        loadScript = transform.parent.parent.GetComponent<LoadScript>();
    }

    public void CloseGallery()
    {
        loadScript.ShowPage(0);
        titleAnimator.SetBool("IsMenuOff", false);
        galleryAnimator.SetBool("IsLoadMenuOpen",false);
    }
}
