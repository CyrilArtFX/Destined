using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Characters.AI
{
	//  Abstract struct for getting a typed value from either StateMachine properties or const value
	public struct AIProperty<T>
	{
		public bool IsProperty;
		public string Key;
		public T Value;

		public AIProperty(string key)
		{
			Key = key;
			Value = default;
			IsProperty = true;
		}

		public AIProperty(T value)
		{
			Key = null;
			Value = value;
			IsProperty = false;
		}

		public void SetValue(AIStateMachine machine, T value)
		{
			if (IsProperty)
			{ 
				machine.SetProperty(Key, value);
				return;
			}

			Value = value;
		}

		public T GetValue(AIStateMachine machine)
		{
			if (IsProperty)
				return machine.GetProperty<T>(Key);

			return Value;
		}
	}

	public sealed class AIStateMachine : MonoBehaviour
	{
		public AIState CurrentState { get; private set; }
		public AIController AIController { get; private set; }

		public readonly Dictionary<string, object> Properties = new();
		public readonly List<AIState> States = new();

		private int currentStateID = 0;

		void Awake()
		{
			AIController = GetComponent<AIController>();
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
			CurrentState = state;
			CurrentState.Start();

			//Debug.Log( "Starting state " + state );
		}

		public bool NextState()
		{
			while (currentStateID < States.Count)
			{
				AIState state = States[currentStateID];
				if (!state.CanRun(state)) 
				{
					currentStateID++;
					continue;
				};

				SetState(state);
				return true;
			}

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