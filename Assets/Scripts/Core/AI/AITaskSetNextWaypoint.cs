using UnityEngine;

namespace Core.AI
{
	public class AITaskSetNextWaypoint : AITask
	{
		public Transform[] Waypoints;
		public int CurrentWaypointID = 0;

		public AIProperty<Vector2> Position;

		public override void OnStart()
		{
			if (Waypoints.Length == 0)
			{
				PrintWarning("no waypoints have been assigned, skipping.");

				End(false);
				return;
			}

			Position.Value = Waypoints[CurrentWaypointID].position;
			CurrentWaypointID = (CurrentWaypointID + 1) % Waypoints.Length;

			End(true);
		}
	}
}