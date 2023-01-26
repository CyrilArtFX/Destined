using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
	public class AISignal
	{
		public Vector2 Position;
	}

	public static class AISignalizer
	{
		public static List<AISignalReceiver> Receivers = new();

		public static void Emit(AISignal signal)
		{
			foreach (AISignalReceiver receiver in Receivers)
			{
				receiver.Receive(signal);
			}
		}
	}
}