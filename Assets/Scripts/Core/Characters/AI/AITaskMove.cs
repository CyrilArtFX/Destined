using Core.Astar2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Characters.AI
{
	public abstract class AITaskMove : AITask
	{
		public AIProperty<float> AcceptanceRadius = new(0.05f);
		public AIProperty<float> SpeedMultiplier = new(1.0f);

		protected List<Node2D> path;
		
		public override void OnTick(float dt)
		{
			if (path == null || path.Count == 0)
			{
				End(true);
				return;
			}

			//  get next node pos
			Vector3 next_pos = path[0].Position;

			//  check is in acceptance radius
			float acceptance_radius = AcceptanceRadius.GetValue(StateMachine);
			if ((next_pos - StateMachine.AIController.transform.position).sqrMagnitude <= acceptance_radius * acceptance_radius)
			{
				path.RemoveAt(0);
			}
			else
			{
				float speed = SpeedMultiplier.GetValue(StateMachine);
				StateMachine.AIController.Mover.MoveTowards(next_pos, speed, speed);
			}
		}

		public bool ComputePathTo(Vector2 pos)
		{
			if (!StateMachine.AIController.Pathfinder.FindPath(pos))
				return false;

			path = StateMachine.AIController.Pathfinder.Path;
			return true;
		}

		public override void OnDrawGizmos()
		{
			if (path == null || path.Count == 0) return;

			Gizmos.color = Color.blue;

			//  draw destination
			Gizmos.DrawWireSphere(path[^1].Position, AcceptanceRadius.GetValue(StateMachine));

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