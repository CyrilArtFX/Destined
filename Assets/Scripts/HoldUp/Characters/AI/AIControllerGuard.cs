using System.Collections;
using UnityEngine;
using Core.AI;

namespace HoldUp.Characters.AI
{
	public class AIControllerGuard : AIController
	{
		public const string TARGET_KEY = "Target";
		public const string MOVE_POS_KEY = "MovePos";
		
		public PlayerController Target { get; private set; }

		public enum State
		{
			Attack,
			Patrol,
			Search,
		}
		public State CurrentState { get; private set; }

		[Header("Patrol")]
		[SerializeField]
		private Transform[] patrolWaypoints;
		[SerializeField]
		private float patrolWaitTime = 3.0f;

		[Header("Search")]
		[SerializeField]
		private LayerMask seeLayerMask;
		[SerializeField]
		private CircleCollider2D seeCollider;

		[Header("Attack")]
		[SerializeField]
		private Weapon weapon;
		[SerializeField]
		private LayerMask fireObstaclesMask;
		[SerializeField]
		private float startAttackReactionTime = 0.4f;
		[SerializeField]
		private float fireDistance = 3.0f;
		[SerializeField, Tooltip("Additional time applied after a bullet was shot to prevent further shooting")]
		private float additionalFireCooldown = 1.0f;
		[SerializeField, Tooltip("For automatic weapons, how much bullets should be shot before applying fire cooldown?")]
		private int fireCooldownForBurst = 6;
		
		private AIProperty<Vector2> movePosProperty;
		private AIProperty<Transform> targetProperty;

		void Start()
		{
			if (weapon != null)
				weapon.Initialize(gameObject, null);

			//  setup AI
			CurrentState = State.Patrol;

			//  setup properties
			movePosProperty = StateMachine.NewProperty<Vector2>("MovePos");
			targetProperty = StateMachine.NewProperty<Transform>("Target");

			AIState state;

			//  create attack state
			state = StateMachine.AddState("Attack");
			state.CanRun = (state) => CurrentState == State.Attack;
			state.AddTasks(
				new AITaskWait()
				{
					Time = new(startAttackReactionTime),
				},
				new AITaskMoveTo()
				{
					NearRadius = new(fireDistance),
					Position = movePosProperty,
				},
				new AITaskFire()
				{
					Weapon = weapon,
					Target = targetProperty,
					AdditionalFireCooldown = new(additionalFireCooldown),
					FireCooldownForBurst = new(fireCooldownForBurst),
					ObstaclesLayerMask = fireObstaclesMask,
				}
			);

			//  create search state
			state = StateMachine.AddState("Search");
			state.CanRun = (state) => CurrentState == State.Search;
			state.OnEnd = (state) => {
				if (state.Status == AIStatus.Failed) return;
				CurrentState = State.Patrol;
			};
			state.AddTasks(
				new AITaskMoveTo()
				{
					Position = movePosProperty,
				},
				new AITaskWait()
				{
					Time = new(3.0f),
				}
			);

			//  create patrol state
			state = StateMachine.AddState("Patrol");
			state.CanRun = (state) => CurrentState == State.Patrol;
			state.AddTasks(
				new AITaskSetNextWaypoint()
				{
					SupressWarning = true,  //  I'm sick of these warnings..
					Waypoints = patrolWaypoints,
					Position = movePosProperty,
				},
				new AITaskMoveTo()
				{
					SpeedMultiplier = new(0.5f),
					Position = movePosProperty,
				},
				new AITaskWait()
				{
					Time = new(patrolWaitTime),
				}
			);
		}

		public void SetTarget(PlayerController target)
		{
			Target = target;
			targetProperty.Value = target != null ? target.transform : null;

			if (target == null)
			{
				CurrentState = State.Search;
			}
		}

		void Update()
		{
			if (Target != null)
			{
				if (Target.Dead)
				{
					SetTarget(null);
					return;
				}

				movePosProperty.Value = Target.transform.position;
			}

			if (weapon != null)
			{
				if (Target != null)
				{
					weapon.SetDirection(Target.transform.position - transform.position);
				}
				else if (Mover.HasMoveThisFrame)
				{
					weapon.SetDirection(-Mover.Direction);
				}
			}
		}

		void OnTriggerStay2D(Collider2D collision)
		{
			if (Target != null && Target.gameObject != collision.gameObject) return;
			if (!collision.TryGetComponent(out PlayerController controller)) return;
			if (controller.Dead) return;

			//  can it see him?
			Vector2 dir = controller.transform.position - transform.position;
			RaycastHit2D[] hits = Physics2D.RaycastAll(seeCollider.transform.position, dir, seeCollider.radius, seeLayerMask);

			bool has_found = false;
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.gameObject == gameObject) 
					continue;
				if (hit.collider != collision)
					break;

				has_found = true;
				break;
			}

			if (!has_found) 
			{
				if (Target != null)
					SetTarget(null);

				return;
			}

			//  attack!
			if (Target == null)
			{
				SetTarget(controller);
				CurrentState = State.Attack;
			}
		}

		public void OnSignalReceive(AISignal signal)
		{
			if (CurrentState != State.Patrol) return;

			CurrentState = State.Search;
			movePosProperty.Value = signal.Position;
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			if (Target != null && Target.gameObject == collision.gameObject)
				SetTarget(null);
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, fireDistance);

			if (StateMachine != null)
			{ 
				Vector2 move_pos = StateMachine.GetProperty<Vector2>(MOVE_POS_KEY);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(move_pos, 0.5f);
				Gizmos.DrawLine(transform.position, move_pos);
			}
		}
	}
}