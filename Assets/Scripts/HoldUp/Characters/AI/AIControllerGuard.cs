using System.Collections;
using UnityEngine;
using Core.Characters.AI;

namespace HoldUp.Characters.AI
{
	public enum AIControllerState
	{
		None,
		Patrol,
		Search,
		Attack,
	}

	public class AIControllerGuard : AIController
	{
		public const string TARGET_KEY = "Target";
		public const string MOVE_POS_KEY = "MovePos";
		public const string PATROL_WAIT_TIME_KEY = "PatrolWaitTime";
		
		public AIControllerState State { get; private set; }

		[SerializeField]
		private Transform[] patrolWaypoints;

		void Start()
		{
			State = AIControllerState.Patrol;

			//  setup properties
			StateMachine.SetProperty(TARGET_KEY, null);
			StateMachine.SetProperty(MOVE_POS_KEY, Vector2.one);
			StateMachine.SetProperty(PATROL_WAIT_TIME_KEY, 3.0f);

			AIState state;

			//  create patrol state
			state = StateMachine.AddState("Patrol");
			state.CanRun = (state) => State == AIControllerState.Patrol;
			state.AddTask(
				new AITaskSetNextWaypoint()
				{
					Waypoints = patrolWaypoints,
					PosKey = MOVE_POS_KEY,
				}
			);
			state.AddTask(
				new AITaskMoveTo()
				{
					SpeedMultiplier = new(0.5f),
					Position = new(MOVE_POS_KEY),
				} 
			);
			state.AddTask(
				new AITaskWait()
				{
					Time = new(PATROL_WAIT_TIME_KEY),
				}
			);

			//  create attack state
			state = StateMachine.AddState("Attack");
			state.CanRun = (state) => State == AIControllerState.Attack;
			state.AddTask(
				new AITaskWait()
				{
					Time = new(1.0f),
				}
			);
		}
	}
}