using UnityEngine;

public class AnimationLesLigneAMettre : MonoBehaviour
{

    private Animator _animator;

    public bool play = false;


    void Start()
    {
        _animator = GetComponent<Animator>();

        //_animator.SetTrigger("Play");
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            _animator.SetTrigger("Play");
        }
    }
}
