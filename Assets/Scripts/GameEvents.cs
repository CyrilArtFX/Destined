using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
	public static UnityEvent<Transform, Collectible> OnCollect = new();
}
