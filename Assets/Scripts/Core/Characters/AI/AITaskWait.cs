using System.Collections;
using UnityEngine;

namespace Core.Characters.AI
{
	public class AITaskWait : AITask
	{
		public AIProperty<float> Time;

		private float currentWaitTime = 0.0f;

		public override void OnStart()
		{
			currentWaitTime = Time.GetValue(StateMachine);
		}

		public override void OnTick(float dt)
		{
			if ((currentWaitTime -= dt) > 0.0f) return;
			
			End(true);
		}
	}
}