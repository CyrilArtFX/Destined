using System.Collections;
using UnityEngine;
using Core.Characters.AI;

namespace HoldUp.Characters.AI
{
	public class AIControllerGuard : AIController
	{
		public const string MOVE_POS_KEY = "MovePos";
		public const string PATROL_WAIT_TIME_KEY = "PatrolWaitTime";
		public const string IS_PATROLLING_KEY = "IsPatrolling";

		[SerializeField]
		private Transform[] patrolWaypoints;

		void Start()
		{
			//  setup properties
			StateMachine.SetProperty(IS_PATROLLING_KEY, true);
			StateMachine.SetProperty(PATROL_WAIT_TIME_KEY, 3.0f);
			StateMachine.SetProperty(MOVE_POS_KEY, Vector2.one);

			//  create patrol state
			AIState state = StateMachine.AddState("Patrol");
			state.CanRun = (state) => StateMachine.GetProperty<bool>(IS_PATROLLING_KEY);
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
					SpeedMultiplier = 0.5f,
					PosKey = MOVE_POS_KEY,
				} 
			);
			state.AddTask(
				new AITaskWait()
				{
					TimeKey = PATROL_WAIT_TIME_KEY,
				}
			);
		}
	}
}