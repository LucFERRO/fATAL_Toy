using UnityEngine;

public class CloseGalleryMenu : MonoBehaviour
{
    Animator galleryAnimator;
    public Animator titleAnimator;

    private void Start()
    {
        galleryAnimator = transform.parent.parent.GetComponent<Animator>();
    }

    public void CloseGallery()
    {
        titleAnimator.SetBool("IsMenuOff", false);
        galleryAnimator.SetBool("IsLoadMenuOpen",false);
    }
}
