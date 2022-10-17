using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    // Paddle components
    private Rigidbody2D rb;

    [Header("Paddle Attributes")]
    [SerializeField] float speed = 1;
    [SerializeField] Vector3 startPosition = Vector3.zero;

    private float movementDirection = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }
    }

    public void Reset()
    {
        transform.position = startPosition;
    }

    private void Update()
    {
        Vector2 moveVectorDir = new Vector2(0, movementDirection * speed * Time.deltaTime);
        rb.MovePosition(rb.position + moveVectorDir);
    }
}