using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core.AI
{
	public class AISignalReceiver : MonoBehaviour
	{
		public UnityEvent<AISignal> OnReceive = new();

		[SerializeField]
		private float MaxReceiveDistance = 3.0f;

		void Start()
		{
			AISignalizer.Receivers.Add(this);
		}

		void OnDestroy()
		{
			AISignalizer.Receivers.Remove(this);
		}

		public bool CanReceive(AISignal signal)
		{
			Vector2 dir = signal.Position - (Vector2) transform.position;
			return dir.sqrMagnitude <= MaxReceiveDistance * MaxReceiveDistance;
		}

		public bool Receive(AISignal signal)
		{
			if (!CanReceive(signal)) return false;

			OnReceive.Invoke(signal);
			return true;
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, MaxReceiveDistance);
		}
	}
}