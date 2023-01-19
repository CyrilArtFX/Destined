using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Core.Characters.AI
{
	public enum AIStatus
	{
		NotStarted,
		Running,
		Success,
		Failed,
	}

	public sealed class AIState
	{
		public AIStatus Status { get; private set; }

		public string Name { get; set; }
		public AIStateMachine StateMachine { get; set; }

		public AITask CurrentTask = null;
		public readonly List<AITask> Tasks = new();

		public Func<AIState, bool> CanRun = (state) => true;

		public void Start()
		{
			if ( !NextTask() ) return; 

			Status = AIStatus.Running;
		}

		public void End( bool is_success )
		{
			Status = is_success ? AIStatus.Success : AIStatus.Failed;

			if ( CurrentTask != null )
			{
				if ( CurrentTask.Status == AIStatus.Running )
					CurrentTask.OnEnd();

				CurrentTask = null;
			} 
		}

		public T AddTask<T>( T task ) where T : AITask
		{
			task.State = this;
			task.StateMachine = StateMachine;
			Tasks.Add( task );

			return task;
		}

		public bool SetTask( AITask task )
		{
			if ( !task.CanRun( task ) )
			{
				End( false );
				return false;
			}
			
			CurrentTask = task;
			CurrentTask.Start();
			
			//Debug.Log( "Starting task " + task );

			return true;
		}

		public bool NextTask()
		{
			int index = 0;
			if ( CurrentTask != null )
				index = Tasks.IndexOf( CurrentTask ) + 1;

			if ( index >= Tasks.Count )
			{
				End( true );
				return false;
			}

			return SetTask( Tasks[index] );
		}

		public void OnTick( float dt )
		{
			if ( CurrentTask != null )
			{
				if ( !CurrentTask.CanRun( CurrentTask ) )
				{
					NextTask();
					return;
				}

				CurrentTask.OnTick( dt );

				if ( CurrentTask.Status == AIStatus.Success )
				{
					NextTask();
				}
				else if ( CurrentTask.Status == AIStatus.Failed )
				{
					End( false );
				}
			}
		}
	}
}
