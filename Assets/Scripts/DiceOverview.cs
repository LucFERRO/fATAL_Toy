using UnityEngine;

public class DiceOverview : MonoBehaviour
{
    private Transform diceTransform;
    public GameManager gameManager;
    public float verticalOffset;

    void Update()
    {
        CheckDice();
        FollowDice();
    }
    private void CheckDice()
    {
        if (gameManager.transform.childCount == 0)
        {
            diceTransform = null;
        } else
        {
            diceTransform = gameManager.transform.GetChild(0).transform;
        }
    }
    private void FollowDice()
    {
        if (diceTransform == null)
        {
            return;
        }
        transform.position = new Vector3(diceTransform.position.x , diceTransform.position.y + verticalOffset, diceTransform.position.z);
    }
}