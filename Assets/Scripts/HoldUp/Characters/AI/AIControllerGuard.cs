using System.Collections;
using UnityEngine;
using Core.Characters.AI;

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

		private AIState attackState, searchState, patrolState;

		void Start()
		{
			if (weapon != null)
				weapon.Initialize(gameObject, null);

			//  setup AI
			CurrentState = State.Patrol;

			//  setup properties
			StateMachine.SetProperty(TARGET_KEY, null);
			StateMachine.SetProperty(MOVE_POS_KEY, Vector2.one);

			//  create attack state
			attackState = StateMachine.AddState("Attack");
			attackState.CanRun = (state) => CurrentState == State.Attack;
			/*attackState.OnEnd = (state) => {
				if (state.Status == AIStatus.Success && Target == null)
				{
					CurrentState = State.Patrol;
				}
			};*/
			attackState.AddTasks(
				new AITaskWait()
				{
					Time = new(startAttackReactionTime),
				},
				new AITaskMoveNearTransform()
				{
					NearRadius = new(fireDistance),
					Target = new(TARGET_KEY),
					OnNullTransform = (state, last_pos) =>
					{
						StateMachine.SetProperty(MOVE_POS_KEY, (Vector2) last_pos);
						CurrentState = State.Search;
					}
				},
				new AITaskFire()
				{
					Weapon = weapon,
					Target = new(TARGET_KEY),
					ObstaclesLayerMask = fireObstaclesMask,
				}
			);

			//  create search state
			searchState = StateMachine.AddState("Search");
			searchState.CanRun = (state) => CurrentState == State.Search;
			searchState.OnEnd = (state) => {
				if (state.Status == AIStatus.Failed) return;
				CurrentState = State.Patrol;
			};
			searchState.AddTasks(
				new AITaskMoveTo()
				{
					Position = new(MOVE_POS_KEY),
				},
				new AITaskWait()
				{
					Time = new(3.0f),
				}
			);

			//  create patrol state
			patrolState = StateMachine.AddState("Patrol");
			patrolState.CanRun = (state) => CurrentState == State.Patrol;
			patrolState.AddTasks(
				new AITaskSetNextWaypoint()
				{
					Waypoints = patrolWaypoints,
					Position = new(MOVE_POS_KEY),
				},
				new AITaskMoveTo()
				{
					SpeedMultiplier = new(0.5f),
					Position = new(MOVE_POS_KEY),
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
			StateMachine.SetProperty(TARGET_KEY, target != null ? target.transform : null);
		}

		void Update()
		{
			if (Target != null)
			{
				if (Target.Dead)
					SetTarget(null);
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
				Target = controller;
				StateMachine.SetProperty(TARGET_KEY, controller.transform);
				CurrentState = State.Attack;
			}
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			if (Target != null && Target.gameObject == collision.gameObject)
				SetTarget(null);
		}
	}
}