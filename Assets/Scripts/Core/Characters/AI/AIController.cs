using Core.Astar2D;
using System.Collections;
using UnityEngine;

namespace Core.Characters.AI
{
	[RequireComponent(typeof(AIStateMachine), typeof(CharacterMover), typeof(Pathfinder2D))]
	public abstract class AIController : MonoBehaviour
	{
		public AIStateMachine StateMachine { get; private set; }
		public CharacterMover Mover { get; private set; }
		public Pathfinder2D Pathfinder { get; private set; }

		protected virtual void Awake()
		{
			StateMachine = GetComponent<AIStateMachine>();
			Mover = GetComponent<CharacterMover>();
			Pathfinder = GetComponent<Pathfinder2D>();
		}
	}
}