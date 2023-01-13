using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Collider2D ) )]
public class Collectible : MonoBehaviour
{
	public enum ItemType
	{
		CARROT,
		GOLD_CARROT,
	}

	public ItemType Type => type;
	public int Score => score;
	public bool CanBeThrown => canBeThrown;

	[SerializeField]
	private ItemType type;
	[SerializeField]
	private float collectDelay = 0.5f;
	[SerializeField]
	private bool canBeThrown = true;
	[SerializeField]
	private int score = 1;

	[SerializeField]
	private new SpriteRenderer renderer;

	private Dictionary<Inventory, float> lastCollectTimes = new();

	private new Collider2D collider;

	void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	public bool Collect( Inventory inventory )
	{
		if ( !CanCollect( inventory ) ) return false;

		lastCollectTimes[inventory] = Time.time + collectDelay;
		collider.enabled = false;
		
		OnCollect( inventory );

		GameEvents.OnCollect.Invoke( inventory.Owner, this );

		renderer.sortingOrder = 1;

		return true;
	}

	public void Drop( Inventory inventory )
	{
		lastCollectTimes[inventory] = Time.time + collectDelay;
		collider.enabled = true;

		OnDrop( inventory );

		renderer.sortingOrder = -1;
	}

	public virtual bool CanCollect( Inventory inventory )
	{
		if ( lastCollectTimes.ContainsKey( inventory ) )
			return lastCollectTimes[inventory] <= Time.time;

		return true;
	}
	public virtual void OnCollect( Inventory inventory ) {}
	public virtual void OnDrop( Inventory inventory ) {}
}