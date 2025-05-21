using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject ResumeButt;
    private Animator m_animator;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_animator = ResumeButt.GetComponent<Animator>();
    }

    public void resumeIsPressed()
    {
        m_animator.SetTrigger("Resume");
    }
}
