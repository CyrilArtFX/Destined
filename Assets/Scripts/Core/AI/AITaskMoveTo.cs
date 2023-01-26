using UnityEngine;

namespace Core.AI
{
	public class AITaskMoveTo : AITaskMove
	{
		public AIProperty<float> NearRadius = new(0.1f);
		public AIProperty<float> RefreshRadius = new(1.0f);
		public AIProperty<Vector2> Position;

		public override void OnTick(float dt)
		{
			Vector3 position = Position.Value;

			//  refresh path
			if (path == null)
			{
				if (!ComputePathTo(position))
				{
					PrintWarning("could not generate path: target position blocked");

					End(false);
					return;
				}
			}

			//  end moving on near enough from target
			float near_radius = NearRadius.Value;
			float near_radius_sqr = near_radius * near_radius;
			float dist_from_target = (StateMachine.AIController.transform.position - position).sqrMagnitude;
			if (dist_from_target <= near_radius_sqr)
			{
				End(true);
				return;
			}

			//  success on empty path
			if (path.Count == 0)
			{
				End(true);
				return;
			}

			//  refresh path if target is too far
			float refresh_radius = RefreshRadius.Value;
			float refresh_radius_sqr = refresh_radius * refresh_radius;
			float dist_end_path_to_target = (path[^1].Position - position).sqrMagnitude;
			if (dist_end_path_to_target > refresh_radius_sqr)
			{
				path = null;
				return;
			}

			base.OnTick( dt );
		}

		public override void OnDrawGizmos()
		{
			base.OnDrawGizmos();

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(Position.Value, NearRadius.Value);
		}
	}
}
