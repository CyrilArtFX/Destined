using System;
using UnityEngine;

namespace Core.AI
{
	public abstract class AITask
	{
		public AIStatus Status { get; private set; }

		public AIState State { get; set; }
		public AIStateMachine StateMachine { get; set; }

		public bool SupressWarning = false;

		public Func<AITask, bool> CanRun = (task) => true;
		public Action<AITask> OnFailed;
		
		public void Start()
		{
			Status = AIStatus.Running;

			OnStart();
		}

		public void End(bool is_success)
		{
			Status = is_success ? AIStatus.Success : AIStatus.Failed;

			if (!is_success)
				OnFailed?.Invoke(this);

			OnEnd();
		}

		public virtual void OnStart() {}
		public virtual void OnTick(float dt) {}
		public virtual void OnEnd() {}
		public virtual void OnDrawGizmos() {}

		private string FormatPrintText(string text) => GetType().FullName + ": " + text;
		public void Print(string text) => Debug.Log(FormatPrintText(text));
		public void PrintWarning(string text) 
		{
			if (!SupressWarning) 
				Debug.LogWarning(FormatPrintText(text));
		}
		public void PrintError(string text) => Debug.LogError(FormatPrintText(text));

		public override string ToString() => GetType().Name;
	}
}
