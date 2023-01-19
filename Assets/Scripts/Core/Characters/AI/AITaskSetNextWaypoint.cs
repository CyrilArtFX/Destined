using UnityEngine;

namespace Core.Characters.AI
{
	public class AITaskSetNextWaypoint : AITask
	{
		public Transform[] Waypoints;
		public int CurrentWaypointID = 0;

		public string PosKey;

		public override void OnStart()
		{
			if (Waypoints.Length == 0)
			{
				Debug.LogWarning(GetType().FullName + ": no waypoints have been assigned, skipping.");

				End(false);
				return;
			}

			StateMachine.SetProperty(PosKey, (Vector2) Waypoints[CurrentWaypointID].position);
			CurrentWaypointID = (CurrentWaypointID + 1) % Waypoints.Length;

			End(true);
		}
	}
}