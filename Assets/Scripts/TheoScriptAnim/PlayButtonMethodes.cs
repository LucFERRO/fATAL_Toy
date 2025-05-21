using UnityEngine;

public class PlayButtonMethodes : MonoBehaviour
{
    public GameObject menu;
    public Animator _MainAnimator;


    private void Start()
    {
        _MainAnimator = menu.GetComponent<Animator>();
    }


    public void PlayButtonPressedAnimation()
    {
        _MainAnimator.SetTrigger("Play");
    }


}
