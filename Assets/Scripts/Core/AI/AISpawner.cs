using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Core.AI
{
	public class AISpawner : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] aiPrefabs;
		[SerializeField]
		private AnimationCurve frequencyCurve;
		[SerializeField]
		private float timePerSpawn = 1.0f;
		[SerializeField]
		private Vector2 spawnSize = new(1.0f, 1.0f);
		[SerializeField]
		private int maxAlive = 10;

		[Header("Events")]
		public UnityEvent<AIController> OnSpawn = new();

		private List<Transform> aliveObjects = new();
		private float spawnTime;
		private float totalTime = 0.0f;

		void Awake()
		{
			spawnTime = timePerSpawn;
		}

		void Update()
		{
			totalTime += Time.deltaTime;

			if ((spawnTime -= Time.deltaTime) <= 0.0f)
			{
				aliveObjects.RemoveAll(x => !x);
				spawnTime = timePerSpawn;

				if (aliveObjects.Count >= maxAlive) return;

				Spawn();
			}
		}

		public int Spawn()
		{
			int count = Mathf.RoundToInt(frequencyCurve.Evaluate(totalTime));
			int next_maximum = Mathf.Min(aliveObjects.Count + count, maxAlive);

			for (int i = aliveObjects.Count; i < next_maximum; i++)
			{
				GameObject obj = Instantiate(RandomUtils.Element(aiPrefabs), GameManager.instance.transform);
				obj.transform.position = transform.position + (Vector3) RandomUtils.CenterOffset(spawnSize);

				aliveObjects.Add(obj.transform);
				
				OnSpawn.Invoke(obj.GetComponent<AIController>());
			}

			return count;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(transform.position, spawnSize);
		}
	}
}