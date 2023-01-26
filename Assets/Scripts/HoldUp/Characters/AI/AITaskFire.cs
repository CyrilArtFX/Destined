using Core.AI;
using UnityEngine;

namespace HoldUp.Characters.AI
{
	public class AITaskFire : AITask
	{
		public Weapon Weapon;
		public LayerMask ObstaclesLayerMask;

		public AIProperty<Transform> Target;
		public AIProperty<float> AdditionalFireCooldown = new(0.5f);
		public AIProperty<int> FireCooldownForBurst = new(6);

		private float additionalFireCooldown = 0.0f;
		private int fireCooldownForBurst = 0;

		public override void OnStart()
		{
			if (Weapon == null)
			{
				End(false);
				return;
			}

			Weapon.OnShoot.AddListener(OnWeaponShoot);
		}

		public override void OnTick( float dt )
		{
			Transform target = Target.Value;
			if (target == null)
			{
				End(true);
				return;
			}

			if ((additionalFireCooldown -= Time.deltaTime) > 0.0f) 
			{
				Weapon.OnUseReleased();
				return;
			}

			Vector3 origin = Weapon.Muzzle.position;
			Vector2 dir = target.position - origin;
			RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, dir.magnitude, ObstaclesLayerMask);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.gameObject == StateMachine.AIController.gameObject) continue;

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

		void OnWeaponShoot(Bullet bullet)
		{
			if (Weapon.IsAutomatic && (fireCooldownForBurst += 1) < FireCooldownForBurst.Value) return;

			additionalFireCooldown = AdditionalFireCooldown.Value;
			fireCooldownForBurst = 0;
		}
	}
}