using UnityEngine;

public abstract class State
{
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();

}