using System.Collections.Generic;
using UnityEngine;
using Core.Astar2D;
using static UnityEngine.GraphicsBuffer;

namespace Core.Characters.AI
{
	public class AITaskMoveTo : AITaskMove
	{
		public AIProperty<Vector2> Position;

		public override void OnStart()
		{
			Vector2 target = Position.GetValue(StateMachine);
			if (!ComputePathTo(target))
				End(false);
		}
	}
}
