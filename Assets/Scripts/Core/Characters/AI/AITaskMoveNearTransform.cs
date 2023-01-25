using UnityEngine;

namespace Core.Characters.AI
{
	public class AITaskMoveNearTransform : AITaskMove
	{
		public AIProperty<float> NearRadius = new(0.5f);
		public AIProperty<float> RefreshRadius = new(1.0f);
		public AIProperty<Transform> Target;
		public AIProperty<Vector3> LastPosition = new(Vector3.zero);

		public bool ShouldFailOnNullTransform = false;

		private Vector3 lastPosition;
		private bool hasLostTarget = false;

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
				hasLostTarget = false;

				lastPosition = target.position;
				LastPosition.SetValue(StateMachine, lastPosition);
			}
			else 
			{
				hasLostTarget = true;

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

			//  success on empty path
			if (path.Count == 0)
			{
				Debug.Log("success from no path");
				End(true);
				return;
			}

			//  end moving on near enough from target
			float near_radius = hasLostTarget ? 0.01f : NearRadius.GetValue(StateMachine);
			float near_radius_sqr = near_radius * near_radius;
			float dist_from_target = (StateMachine.AIController.transform.position - lastPosition).sqrMagnitude;
			if (dist_from_target <= near_radius_sqr)
			{
				Debug.Log("success from near radius");
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