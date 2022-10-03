using UnityEngine;
using UnityEngine.Events;

public class GoalTrigger : MonoBehaviour
{

    public UnityEvent OnGoalTouch = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() != null)
        {
            OnGoalTouch.Invoke();
        }
    }
}
