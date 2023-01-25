using Core.Characters.AI;
using UnityEngine;

namespace HoldUp.Characters.AI
{
	public class AITaskFire : AITask
	{
		public Weapon Weapon;
		public LayerMask ObstaclesLayerMask;
		public AIProperty<Transform> Target;

		public override void OnStart()
		{
			if (Weapon == null)
				End(false);
		}

		public override void OnTick( float dt )
		{
			Transform target = Target.GetValue(StateMachine);
			if (target == null)
			{
				End(true);
				return;
			}

			Vector3 origin = StateMachine.AIController.transform.position;
			Vector2 dir = target.position - origin;
			RaycastHit2D hit = Physics2D.Raycast(origin, dir, dir.magnitude, ObstaclesLayerMask);
			if (hit)
			{
				Weapon.OnUseReleased();
				return;
			}

			Weapon.OnUsePressed();
		}

		public override void OnEnd()
		{
			if (Weapon != null)
				Weapon.OnUseReleased();
		}
	}
}