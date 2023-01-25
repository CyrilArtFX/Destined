using System;

namespace Core.Characters.AI
{
	public abstract class AITask
	{
		public AIStatus Status { get; private set; }

		public AIState State { get; set; }
		public AIStateMachine StateMachine { get; set; }

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

		public override string ToString() => GetType().Name;
	}
}
