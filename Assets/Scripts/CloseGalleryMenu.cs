using UnityEngine;

public class CloseGalleryMenu : MonoBehaviour
{
    Animator galleryAnimator;

    private void Start()
    {
        galleryAnimator = transform.parent.parent.GetComponent<Animator>();
    }

    public void CloseGallery()
    {
        galleryAnimator.SetBool("IsLoadMenuOpen",false);
    }
}
