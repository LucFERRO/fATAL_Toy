using UnityEngine;

public struct PhysicalDiceProperties
{
    public Vector3 direction;
    public Vector3 initialPosition;
    public float initialSpeed;
    public float mass;
    public float drag;
    public bool IsValid()
    {
        //Debug.Log($"Direction {direction}");
        //Debug.Log($"InitialPosition {initialPosition}");
        //Debug.Log($"initialSpeed {initialSpeed}");
        //Debug.Log($"mass {mass}");
        //Debug.Log($"drag {drag}");
        return !float.IsNaN(initialPosition.x) && !float.IsNaN(initialPosition.y) && !float.IsNaN(initialPosition.z) &&
               !float.IsNaN(direction.x) && !float.IsNaN(direction.y) && !float.IsNaN(direction.z) &&
               initialSpeed > 0 && mass > 0 && drag >= 0;
    }
}