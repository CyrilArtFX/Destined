﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static UnityEditor.Progress;

[RequireComponent( typeof( CircleCollider2D ) )]
public class Inventory : MonoBehaviour
{
	public Player Player => player;
	public ReadOnlyCollection<Collectible> Items => items.AsReadOnly();
	public int ItemsCount => items.Count;

	[SerializeField]
	private float itemOffset = 0.25f;
	[SerializeField]
	private float dropRadius = 0.25f;
	[SerializeField]
	private Player player;

	private readonly List<Collectible> items = new();

	public bool AddItem( Collectible item )
	{
		if ( items.Contains( item ) ) return false;
		if ( !item.Collect( this ) ) return false;

		items.Add( item );

		//  parent
		item.transform.SetParent( transform );
		UpdateItemPosition( item );

		return true;
	}

	public void UpdateItemPosition( Collectible item )
	{
		item.transform.localPosition = new Vector3( 0.0f, itemOffset * items.Count );
		item.transform.localEulerAngles = Vector3.zero;
	}

	public void UpdateItemsPositions()
	{
		foreach ( Collectible item in items )
			UpdateItemPosition( item );
	}

	public void DropItem( Collectible item )
	{
		if ( !items.Contains( item ) ) return;

		//  drop event
		item.Drop( this );

		//  remove from container
		items.Remove( item );

		//  un-parent
		item.transform.SetParent( null );

		//  set pos
		float ang = Random.Range( 0.0f, 360.0f );
		item.transform.position = transform.position + new Vector3( Mathf.Cos( ang ) * dropRadius, Mathf.Sin( ang ) * dropRadius );
		item.transform.localEulerAngles = Vector3.zero;
	}

	public void DropLastItem()
	{
		if ( items.Count <= 0 ) return;

		DropItem( items[items.Count - 1] );
	}

	public void RemoveItem( Collectible item )
	{
		if ( !items.Contains( item ) ) return;

		//  remove from container
		items.Remove( item );

		//  destroy item
		Destroy( item.gameObject );
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if ( !collision.TryGetComponent( out Collectible item ) ) return;
		
		AddItem( item );
	}
}