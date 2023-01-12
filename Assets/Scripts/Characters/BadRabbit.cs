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

	private Player playerTarget;
	private Vector3 moveTarget;
	private Vector3 moveDir;
	private bool isIdling = true;
	private float currentIdleTime = 0.0f;
	private State state = State.PATROL;

	[Header( "References" )]
	[SerializeField]
	private Animator animator;
	[SerializeField]
	private Rabbit_AnimController animController;

	void Start()
	{
		moveTarget = transform.position;
		currentIdleTime = idleTimeRange.x;

		animController.OnAnimAttack = OnAnimAttack;
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
				break;
		}
		
		//  movement
		Vector3 dir = moveTarget - transform.position;
		if ( !isIdling && animController.CanMove )
		{
			float speed = Time.deltaTime * ( state == State.ATTACK ? chaseSpeed : moveSpeed );
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
		}

		animator.SetBool( "IsWalking", !isIdling );
		animator.SetFloat( "AnimSpeed", state == State.ATTACK ? chaseSpeed / moveSpeed : 1.0f );
	}

	void GameEventOnCollect( Player player, Collectible item )
	{
		//  check is within territory
		float dist_sqr = ( player.transform.position - territoryCenter.position ).sqrMagnitude;
		if ( dist_sqr > territoryRadius * territoryRadius )
			return;

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

	void SetPlayerTarget( Player player )
	{
		playerTarget = player;
		state = player == null ? State.PATROL : State.ATTACK;

		isIdling = false;
		ResetIdle();
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
		animator.SetBool( "Attack", false );
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
					animator.SetBool( "Attack", true );
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
	}
}
