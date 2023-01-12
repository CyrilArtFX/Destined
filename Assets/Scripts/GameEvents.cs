using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
	public static UnityEvent<Player, Collectible> OnCollect = new();
}
