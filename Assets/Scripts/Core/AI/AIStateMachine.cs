using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.AI
{
	//  Abstract struct for getting a typed value from either StateMachine properties or const value
	public struct AIProperty<T>
	{
		public bool IsProperty => Key != null;

		public string Key;

		public T Value
		{
			get 
			{
				if (IsProperty) 
					return StateMachine.GetProperty<T>(Key);

				return storedValue;
			}
			set
			{
				if (IsProperty)
					StateMachine.SetProperty(Key, value);
				else
					storedValue = value;
			}
		}
		private T storedValue;

		public AIStateMachine StateMachine;

		public AIProperty(AIStateMachine machine, string key)
		{
			Key = key;
			storedValue = default;
			StateMachine = machine;
		}

		public AIProperty(AIStateMachine machine, string key, T value) : this(machine, key)
		{
			Value = value;
		}

		public AIProperty(T value)
		{
			Key = null;
			storedValue = value;
			StateMachine = null;
		}
	}

	public sealed class AIStateMachine : MonoBehaviour
	{
		public AIState CurrentState { get; private set; }
		public AIController AIController { get; private set; }

		public readonly Dictionary<string, object> Properties = new();
		public readonly List<AIState> States = new();

		private int currentStateID = -1;

		void Awake()
		{
			AIController = GetComponent<AIController>();
		}

		public AIProperty<T> NewProperty<T>(string key)
		{
			return new(this, key);
		}
		public AIProperty<T> NewProperty<T>(string key, T value)
		{
			return new(this, key, value);
		}
		public void SetProperty(string key, object value) => Properties[key] = value;
		public T GetProperty<T>(string key) 
		{
			if (Properties.TryGetValue(key, out object value))
			{
				if (value != null)
				{
					if (value is T casted)
					{
						return casted;
					}
					else
					{
						Debug.LogWarning(GetType().FullName + ": can't cast property '" + key + "' of type '" + value.GetType().FullName + "' to " + typeof(T).FullName + "'");
					}
				}
			}
			else
			{
				Debug.LogWarning(GetType().FullName + ": property '" + key + "' doesn't exists!");
			}
		
			return default;
		}

		public AIState AddState(string name)
		{
			AIState state = new()
			{
				Name = name,
				StateMachine = this,
			};

			States.Add(state);
			return state;
		}

		public void SetState(AIState state) 
		{
			if (CurrentState?.Status == AIStatus.Running)
				CurrentState.End(false);

			CurrentState = state;
			CurrentState.Start();

			//Debug.Log( "Starting state " + state );
		}

		public bool NextState()
		{
			while (currentStateID + 1 < States.Count)
			{
				AIState state = States[++currentStateID];
				if (!state.AutoRun || !state.CanRun(state)) 
					continue;

				SetState(state);
				return true;
			}

			currentStateID = -1;
			return false;
		}

		void Update()
		{
			if (CurrentState != null)
			{
				if (!CurrentState.CanRun(CurrentState))
				{
					NextState();
					return;
				}

				CurrentState.OnTick(Time.deltaTime);

				if (CurrentState.Status == AIStatus.Success || CurrentState.Status == AIStatus.Failed)
				{
					NextState();
				}
			}
			else
			{
				NextState();
			}
		}

		void OnDrawGizmosSelected()
		{
			CurrentState?.CurrentTask?.OnDrawGizmos();
		}
	}
}