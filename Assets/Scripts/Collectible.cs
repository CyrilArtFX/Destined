﻿using UnityEngine;

[RequireComponent( typeof( Collider2D ) )]
public class Collectible : MonoBehaviour
{
	[SerializeField]
	private float collectDelay = 0.5f;

	private float lastCollectTime;

	private new Collider2D collider;

	void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	public bool Collect( Inventory inventory )
	{
		if ( !CanCollect( inventory ) ) return false;

		lastCollectTime = Time.time + collectDelay;
		collider.enabled = false;
		
		OnCollect( inventory );
		return true;
	}

	public void Drop( Inventory inventory )
	{
		lastCollectTime = Time.time + collectDelay;
		collider.enabled = true;

		OnDrop( inventory );
	}

	public virtual bool CanCollect( Inventory inventory )
	{
		return lastCollectTime <= Time.time;
	}
	public virtual void OnCollect( Inventory inventory ) {}
	public virtual void OnDrop( Inventory inventory ) {}
}