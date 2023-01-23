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

		[SerializeField]
		private Transform[] patrolWaypoints;
		[SerializeField]
		private LayerMask seeLayerMask;
		[SerializeField]
		private CircleCollider2D seeCollider;
		[SerializeField]
		private float patrolWaitTime = 3.0f;

		void Start()
		{
			//  setup properties
			StateMachine.SetProperty(TARGET_KEY, null);
			StateMachine.SetProperty(MOVE_POS_KEY, Vector2.one);

			AIState state;

			//  create attack state
			state = StateMachine.AddState("Attack");
			state.CanRun = (state) => Target != null;
			state.AddTasks(
				new AITaskWait()
				{
					Time = new(0.4f),
				}
			);

			//  create patrol state
			state = StateMachine.AddState("Patrol");
			state.CanRun = (state) => Target == null;
			state.AddTasks(
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

		void OnTriggerStay2D(Collider2D collision)
		{
			if (Target != null && Target.gameObject != collision.gameObject) return;
			if (!collision.TryGetComponent(out PlayerController controller)) return;

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
			}
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			if (Target != null && Target.gameObject == collision.gameObject)
				SetTarget(null);
		}
	}
}