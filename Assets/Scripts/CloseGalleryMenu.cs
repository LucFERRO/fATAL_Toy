using UnityEngine;

public class CloseGalleryMenu : MonoBehaviour
{
    Animator galleryAnimator;

    private void Start()
    {
        galleryAnimator = transform.parent.GetComponent<Animator>();
    }

    public void CloseGallery()
    {
        galleryAnimator.SetBool("IsLoadMenuOpen",false);
    }
}
