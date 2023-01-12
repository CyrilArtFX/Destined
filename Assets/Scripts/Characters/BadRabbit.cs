using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class BadRabbit : MonoBehaviour
{
	enum State
	{
		PATROL,
		ATTACK,
	}

	[Header( "Behaviour" )]
	[SerializeField]
	private float moveSpeed = .1f;
	[SerializeField]
	private float chaseSpeed = .15f;
	[SerializeField]
	private Transform territoryCenter;
	[SerializeField]
	private float territoryRadius = 5.0f;
	[SerializeField]
	private float moveAcceptanceRadius = 0.1f;
	[SerializeField]
	private Vector2 idleTimeRange = new( 0.8f, 1.2f );
	[SerializeField]
	private float stunTime = 1.0f;

	[Header( "Charge Attack" )]
	[SerializeField]
	private float chargeMoveSpeed = .3f;
	[SerializeField]
	private float chargeStunTime = 1.0f;
	[SerializeField]
	private float chargeRadius = 3.5f;
	[SerializeField]
	private float chargeCooldown = 2.0f;

	[Header( "References" )]
	[SerializeField]
	private Animator animator;
	[SerializeField]
	private Rabbit_AnimController animController;

	private Player playerTarget;
	private Vector3 moveTarget;
	private Vector3 moveDir;
	private bool isIdling = true;
	private float currentIdleTime = 0.0f;
	private float currentChargeCooldown = 0.0f;
	private State state = State.PATROL;

	void Start()
	{
		moveTarget = transform.position;
		currentIdleTime = idleTimeRange.x;

		animController.OnAttack = OnAnimAttack;
		animController.OnChargeJumpEnd = OnAnimChargeJumpEnd;

		GameEvents.OnCollect.AddListener( GameEventOnCollect );
	}

	void Update()
	{
		switch ( state )
		{
			case State.ATTACK:
				if ( playerTarget != null )
					moveTarget = playerTarget.transform.position;
				else
				{
					state = State.PATROL;
					ResetIdle();
				}

				//  decrease charge cooldown
				if ( !animController.IsChargeJumping )
					currentChargeCooldown -= Time.deltaTime;
				break;
		}
		
		//  movement
		Vector3 dir = moveTarget - transform.position;
		if ( !isIdling && animController.CanMove )
		{
			float speed = Time.deltaTime;
			if ( state == State.ATTACK )
				speed *= animController.IsChargeJumping ? chargeMoveSpeed : chaseSpeed;
			else
				speed *= moveSpeed;

			transform.position = transform.position + speed * moveDir;
		}
		//  destination reached
		else if ( dir.magnitude <= moveAcceptanceRadius )
		{
			OnMoveUpdateEnd();
		}
		//  get next movement direction
		else
		{
			moveDir = dir.normalized;

			switch ( state )
			{
				case State.ATTACK:
					if ( ( playerTarget.transform.position - transform.position ).sqrMagnitude > chargeRadius * chargeRadius )
					{
						if ( currentChargeCooldown <= 0.0f )
						{
							animator.SetBool( "IsChargeJumping", true );
							currentChargeCooldown = chargeCooldown;
						}
					}
					break;
			}
		}

		animator.SetBool( "IsWalking", !isIdling );
		animator.SetFloat( "AnimSpeed", state == State.ATTACK ? chaseSpeed / moveSpeed : 1.0f );
	}

	void SetPlayerTarget( Player player )
	{
		playerTarget = player;
		state = player == null ? State.PATROL : State.ATTACK;

		isIdling = false;
		ResetIdle();
	}

	void TryChangePriorityPlayer( Player player )
	{
		if ( player.Controller.IsStun || player.Controller.IsStunImmune ) return;

		//  check priority w/ current player 
		if ( playerTarget != null && playerTarget != player )
		{
			//  priority on the nearest
			float current_dist_sqr = ( player.transform.position - playerTarget.transform.position ).sqrMagnitude;
			if ( ( player.transform.position - transform.position ).sqrMagnitude > current_dist_sqr )
				return;
		}

		//  set new target
		SetPlayerTarget( player );
	}

	void ResetIdle()
	{
		isIdling = false;
		currentIdleTime = Random.Range( idleTimeRange.x, idleTimeRange.y );
	}

	void OnAnimAttack()
	{
		if ( playerTarget == null ) return;

		playerTarget.Controller.Stun( stunTime );
		SetPlayerTarget( null );
		
		isIdling = false;
		animator.ResetTrigger( "Attack" );
	}

	void OnAnimChargeJumpEnd()
	{
		//  stun in circle
		foreach ( Collider2D collider in Physics2D.OverlapCircleAll( transform.position, chargeRadius ) )
		{
			if ( !collider.TryGetComponent( out Player player ) ) continue;

			player.Controller.Stun( chargeStunTime );
		}

		SetPlayerTarget( null );

		isIdling = false;
		animator.SetBool( "IsChargeJumping", false );
	}

	void OnMoveUpdateEnd()
	{
		switch ( state )
		{
			//  attack player
			case State.ATTACK:
				if ( playerTarget != null && !isIdling )
				{
					isIdling = true;
					animator.SetTrigger( "Attack" );
				}
				break;
			//  idling after movement
			case State.PATROL:
				isIdling = true;

				if ( ( currentIdleTime -= Time.deltaTime ) <= 0.0f )
				{
					moveTarget = FindNextPatrolPosition();
					ResetIdle();
				}
				break;
		}
	}

	Vector3 FindNextPatrolPosition()
	{
		Vector3 pos = Vector3.zero;

		for ( int iter = 0; iter < 3; iter++ )
		{
			pos = territoryCenter.position;

			//  get a random position in circle
			float ang = Random.Range( 0.0f, 360.0f );
			float radius = Random.Range( territoryRadius * 0.1f, territoryRadius );
			pos.x += Mathf.Cos( ang ) * radius;
			pos.y += Mathf.Sin( ang ) * radius;

			//  search for a new position if too close
			if ( ( pos - transform.position ).magnitude > moveAcceptanceRadius * 2 )
				break;
		}

		return pos;
	}

	void GameEventOnCollect( Player player, Collectible item )
	{
		//  check is within territory
		float dist_sqr = ( player.transform.position - territoryCenter.position ).sqrMagnitude;
		if ( dist_sqr > territoryRadius * territoryRadius )
			return;

		TryChangePriorityPlayer( player );
	}

	void OnTriggerEnter2D( Collider2D collision )
	{
		if ( !collision.TryGetComponent( out Player player ) ) return;

		TryChangePriorityPlayer( player );
	}

	void OnDrawGizmos()
	{
		//  move target
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( moveTarget, moveAcceptanceRadius );

		//  territory
		if ( territoryCenter != null )
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere( territoryCenter.position, territoryRadius );
		}

		//  player target
		if ( playerTarget != null )
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere( playerTarget.transform.position, 1.0f );
		}

		//  charge
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere( transform.position, chargeRadius );
	}
}
