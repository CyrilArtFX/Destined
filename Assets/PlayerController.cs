using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 Direction { get; private set; }

    [SerializeField]
    private float MoveSpeed = 5.0f;

    private new Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove( InputAction.CallbackContext ctx )
    {
        Direction = ctx.action.ReadValue<Vector2>();
    }

    void Update()
    {
        rigidbody.MovePosition( rigidbody.position + Direction * MoveSpeed / 100.0f );
    }
}
