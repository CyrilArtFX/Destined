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

	[SerializeField]
	private Transform throwStart;

	[SerializeField]
	private GameObject carrotProjectile;

	[SerializeField]
	private float stunImmuneTime;


	private bool throwing = false;
	private float stun = 0.0f;
	private bool stunImmune;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void OnMove( InputAction.CallbackContext ctx )
	{
		Direction = ctx.action.ReadValue<Vector2>();
	
		animator.SetBool( "IsWalking", Direction != Vector2.zero && stun <= 0.0f );
	}

	public void OnDrop( InputAction.CallbackContext ctx )
	{
		if ( !ctx.action.triggered ) return;

		player.Inventory.DropLastItem();
	}

	public void OnThrowController( InputAction.CallbackContext ctx )
	{
		if (player.Inventory.Items.Count == 0) return;

		Vector2 throw_direction = ctx.action.ReadValue<Vector2>();

        if (throw_direction.magnitude >= 1.0f)
		{
			if(!throwing)
			{
				throwing = true;
				throw_direction.Normalize();

				SpawnProjectile(throw_direction);
			}
		}
		else if(throw_direction.magnitude <= 0.3f)
		{
			throwing = false;
		}
	}

	public void OnThrowMouse( InputAction.CallbackContext ctx )
    {
        if (player.Inventory.Items.Count == 0) return;

        if (!ctx.action.triggered) return;

		Vector2 mouse_pos = Mouse.current.position.ReadValue();
		mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

		Vector2 throw_direction = mouse_pos - (Vector2)transform.position;
		throw_direction.Normalize();

		SpawnProjectile(throw_direction);
    }

	private void SpawnProjectile(Vector2 direction)
    {
        GameObject projectile = GameObject.Instantiate(carrotProjectile);
		projectile.transform.position = throwStart.position;
		projectile.GetComponent<CarrotProjectile>().Initialize(direction, gameObject);

		player.Inventory.RemoveItem(player.Inventory.Items[0]);
		player.Inventory.UpdateItemsPositions();
    }

	public void Stun(float stunTime)
	{
		if (stunImmune) return;

		stun = stunTime;
		StartCoroutine(StunImmunity());
	}

	void Update()
	{
		if(stun > 0.0f)
		{
			stun -= Time.deltaTime;
			return;
		}

		rigidbody.MovePosition( rigidbody.position + Direction * moveSpeed );
	}

	private IEnumerator StunImmunity()
	{
		stunImmune = true;
		yield return new WaitForSeconds(stunImmuneTime);
		stunImmune = false;
	}
}
