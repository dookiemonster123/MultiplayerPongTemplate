using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    // Ball components
    private Rigidbody2D rb;

    [Header("Ball Attributes")]
    [SerializeField] float speed;
    [SerializeField] Vector3 startPosition = Vector3.zero;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    public void Reset()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Launches the ball in a random direction.
    /// </summary>
    /// <param name="launchRight">
    /// A bool which determines if the ball goes right (if true) or left (if false), defaults to true
    /// </param>
    public void LaunchBall(bool launchRight = true)
    {
        // launchRight ? 1 : -1 means if launchRight is true then use 1 else use -1
        Vector2 direction = new Vector2(launchRight ? 1 : -1,
                                        Random.Range(-2f, 2f)
                                       ).normalized;
        rb.velocity = direction * speed;
    }
}