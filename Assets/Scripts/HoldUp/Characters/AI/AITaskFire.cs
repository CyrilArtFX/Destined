using Core.Characters.AI;
using UnityEngine;

namespace HoldUp.Characters.AI
{
	public class AITaskFire : AITask
	{
		public Weapon Weapon;
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

			Weapon.OnUsePressed();
		}

		public override void OnEnd()
		{
			if (Weapon != null)
				Weapon.OnUseReleased();
		}
	}
}