using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    // Paddle components
    private Rigidbody2D rb;

    [Header("Paddle Attributes")]
    [SerializeField] float speed;
    [SerializeField] Vector3 startPosition = Vector3.zero;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }
    }

    private void OnEnable()
    {
        transform.position = startPosition;
    }

    private void Update()
    {

    }

    public void UpdateMovement(bool moveUp = true)
    {
        Vector2 direction = moveUp ? Vector2.up : Vector2.down;
        rb.velocity = direction * speed;
    }
}