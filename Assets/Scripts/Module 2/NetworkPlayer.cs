using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayer : NetworkBehaviour
{
    // Paddle components
    private Rigidbody2D rb;

    [Header("Paddle Attributes")]
    [SerializeField] float speed = 10;
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
        if (!IsOwner && IsServer) return;
        transform.position = startPosition;
    }

    private void Update()
    {
        if (!IsOwner) return;
        Vector2 moveVectorDir = new Vector2(0, movementDirection * speed * Time.deltaTime);
        rb.MovePosition(rb.position + moveVectorDir);
    }
    public void UpdateMovement(InputAction.CallbackContext context) {
        if (!IsOwner) return;
        movementDirection = context.ReadValue<float>();
    }

    public void UpdateStartPosition(Vector3 newPosition) {
        startPosition = newPosition;
        transform.position = startPosition;
    }
}
