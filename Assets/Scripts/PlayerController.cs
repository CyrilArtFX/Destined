using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public Vector2 Direction { get; private set; }

	[SerializeField]
	private float moveSpeed = 5.0f;
	[SerializeField]
	private Player player;
	[SerializeField]
	private Animator animator;

	private new Rigidbody2D rigidbody;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void OnMove( InputAction.CallbackContext ctx )
	{
		Direction = ctx.action.ReadValue<Vector2>();
	
		animator.SetBool( "IsWalking", Direction != Vector2.zero );
	}

	public void OnDrop( InputAction.CallbackContext ctx )
	{
		if ( !ctx.action.triggered ) return;

		player.Inventory.DropLastItem();
	}

	void Update()
	{
		rigidbody.MovePosition( rigidbody.position + Direction * moveSpeed );
	}
}
