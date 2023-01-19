using System.Collections;
using UnityEngine;

namespace Core.Characters.AI
{
	public class AITaskWait : AITask
	{
		public string TimeKey;

		private float currentWaitTime = 0.0f;

		public override void OnStart()
		{
			currentWaitTime = StateMachine.GetProperty<float>(TimeKey);
		}

		public override void OnTick(float dt)
		{
			if ((currentWaitTime -= dt) > 0.0f) return;
			
			End(true);
		}
	}
}