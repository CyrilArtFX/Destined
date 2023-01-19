using System.Collections.Generic;
using UnityEngine;
using Core.Astar2D;

namespace Core.Characters.AI
{
	public class AITaskMoveTo : AITask
	{
		public float AcceptanceRadius = 0.05f;
		public float SpeedMultiplier = 1.0f;
		public string PosKey;

		private Vector2 target;
		private List<Node2D> path;

		public override void OnStart()
		{
			target = StateMachine.GetProperty<Vector2>(PosKey);

			StateMachine.AIController.Pathfinder.FindPath(target);
			path = StateMachine.AIController.Pathfinder.Path;
		}

		public override void OnTick(float dt)
		{
			if (path.Count == 0)
			{
				End(true);
				return;
			}

			//  get next node pos
			Vector3 next_pos = path[0].Position;

			//  check is in acceptance radius
			if ((next_pos - StateMachine.AIController.transform.position).sqrMagnitude <= AcceptanceRadius * AcceptanceRadius)
			{
				path.RemoveAt(0);
			}
			else
			{
				StateMachine.AIController.Mover.MoveTowards(next_pos, SpeedMultiplier, SpeedMultiplier);
			}
		}

		public override void OnDrawGizmos()
		{
			if (path == null || path.Count == 0) return;

			Gizmos.color = Color.blue;

			//  draw destination
			Gizmos.DrawWireSphere(path[path.Count - 1].Position, AcceptanceRadius);

			//  draw path
			Vector3 previous_pos = path[0].Position;
			for (int i = 1; i < path.Count; i++)
			{
				Vector2 next_pos = path[i].Position;
				Gizmos.DrawLine(previous_pos, next_pos);
				previous_pos = next_pos;
			}
		}
	}
}
