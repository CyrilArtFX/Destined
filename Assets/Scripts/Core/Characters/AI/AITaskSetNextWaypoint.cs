using UnityEngine;

namespace Core.Characters.AI
{
	public class AITaskSetNextWaypoint : AITask
	{
		public Transform[] Waypoints;
		public int CurrentWaypointID = 0;
		public bool SupressWarning = false;

		public AIProperty<Vector2> Position;

		public override void OnStart()
		{
			if (Waypoints.Length == 0)
			{
				if (!SupressWarning)
					Debug.LogWarning(GetType().FullName + ": no waypoints have been assigned, skipping.");

				End(false);
				return;
			}

			Position.SetValue(StateMachine, (Vector2) Waypoints[CurrentWaypointID].position);
			CurrentWaypointID = (CurrentWaypointID + 1) % Waypoints.Length;

			End(true);
		}
	}
}