using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BadRabbit : MonoBehaviour
{
    [Header( "Behaviour" )]
    [SerializeField]
    private float moveSpeed = .1f;
    [SerializeField]
    private Transform territoryCenter;
    [SerializeField]
    private float territoryRadius = 5.0f;
    [SerializeField]
	private float moveAcceptanceRadius = 0.1f;
    [SerializeField]
	private Vector2 idleTimeRange = new( 0.8f, 1.2f );

    private Vector3 moveTarget;
    private Vector3 moveDir;
    private bool isIdling = true;
    private float currentIdleTime = 0.0f;

    [Header( "References" )]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rabbit_AnimController animController;

	void Start()
    {
        moveTarget = transform.position;
        currentIdleTime = idleTimeRange.x;
    }

	void Update()
    {
        Vector3 dir = moveTarget - transform.position;
        if ( !isIdling && animController.CanMove )
        {
            float speed = Time.deltaTime * moveSpeed;
            transform.position = transform.position + speed * moveDir;
        }
        else if ( dir.magnitude <= moveAcceptanceRadius )
        {
            isIdling = true;

            if ( ( currentIdleTime -= Time.deltaTime ) <= 0.0f )
            {
                moveTarget = FindNextPatrolPosition();

                isIdling = false;
                currentIdleTime = Random.Range( idleTimeRange.x, idleTimeRange.y );
            }
        }
        else 
        {
            moveDir = dir.normalized;
        }

        animator.SetBool( "IsWalking", !isIdling );
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
	}
}
