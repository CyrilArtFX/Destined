using System;
using UnityEngine;

namespace Core.AI
{
	public class AITaskMoveNearTransform : AITaskMove
	{
		public AIProperty<float> NearRadius = new(0.5f);
		public AIProperty<float> RefreshRadius = new(1.0f);
		public AIProperty<Transform> Target;
		public AIProperty<Vector3> LastPosition = new(Vector3.zero);

		public Action<AITaskMoveNearTransform, Vector3> OnNullTransform;

		private Vector3 lastPosition;

		public override void OnStart()
		{
			if (Target.GetValue(StateMachine) == null)
				End(false);
		}

		public override void OnTick(float dt)
		{
			//  check target validity
			Transform target = Target.GetValue(StateMachine);
			if (target != null)
			{
				lastPosition = target.position;
				LastPosition.SetValue(StateMachine, lastPosition);
			}
			else
			{
				OnNullTransform?.Invoke(this, lastPosition);

				End(false);
				return;
			}
			
			//  refresh path
			if (path == null)
			{
				if (!ComputePathTo(lastPosition))
				{
					End(false);
					return;
				}
			}

			//  end moving on near enough from target
			float near_radius = NearRadius.GetValue(StateMachine);
			float near_radius_sqr = near_radius * near_radius;
			float dist_from_target = (StateMachine.AIController.transform.position - lastPosition).sqrMagnitude;
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
			float refresh_radius = RefreshRadius.GetValue(StateMachine);
			float refresh_radius_sqr = refresh_radius * refresh_radius;
			float dist_end_path_to_target = (path[^1].Position - lastPosition).sqrMagnitude;
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
			Gizmos.DrawWireSphere(lastPosition, NearRadius.GetValue(StateMachine));
		}
	}
}