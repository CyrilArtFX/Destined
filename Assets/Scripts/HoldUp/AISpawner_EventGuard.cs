using Core.AI;
using HoldUp.Characters.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace HoldUp
{
	public class AISpawner_EventGuard : MonoBehaviour
	{
		[SerializeField]
		private Transform[] searchPoints;

		Transform[] GenerateWaypoints()
		{
			List<Transform> waypoints = new(searchPoints);
			waypoints.RemoveAll(x => waypoints.Count > 2 && Random.Range(0, 2) == 0);
			return waypoints.ToArray();
		}

		public void OnSpawn(AIController controller)
		{
			AIControllerGuard guard = controller as AIControllerGuard;
			if (guard == null) return;

			guard.SetPatrolWaypoints(GenerateWaypoints());

			StartCoroutine(Coroutine_SearchAtNextFrame(guard));
		}

		IEnumerator Coroutine_SearchAtNextFrame(AIControllerGuard guard)
		{
			yield return new WaitForEndOfFrame();

			guard.SearchAt(RandomUtils.Element(searchPoints).position);
		}
	}
}