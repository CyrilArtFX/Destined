using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burrow : MonoBehaviour
{
	/*[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private new SpriteRenderer renderer;

	void Start()
	{
		renderer.sprite = sprites[Random.Range( 0, sprites.Length )];
	}*/
	[SerializeField]
	GameObject carrotPrefab;
	[SerializeField]
	int maxAliveCarrots = 20;

	[SerializeField]
	float[] zonesRadiuses;
	[SerializeField]
	float[] zonesCooldowns;

	float[] currentZonesCooldowns;
	List<Transform> aliveCarrots = new();

	void Start()
	{
		//  init cooldowns
		currentZonesCooldowns = new float[zonesCooldowns.Length];
		for ( int id = 0; id < currentZonesCooldowns.Length; id++ )
			currentZonesCooldowns[id] = zonesCooldowns[id];
	}

	void Update()
	{
		//  remove null references
		for ( int i = 0; i < aliveCarrots.Count; i++ )
		{
			if ( aliveCarrots[i] == null )
			{
				aliveCarrots.RemoveAt( i );
				break;
			}
		}

		//  handle cooldowns
		for ( int id = 0; id < currentZonesCooldowns.Length; id++ )
		{
			if ( aliveCarrots.Count + 1 > maxAliveCarrots ) continue;
			if ( ( currentZonesCooldowns[id] -= Time.deltaTime ) > 0.0f ) continue;

			SpawnCarrot( GetZoneRange( id ) );
			currentZonesCooldowns[id] = zonesCooldowns[id];
		}
	}

	void SpawnCarrot( Vector2 radius_range )
	{
		//  instantiate
		GameObject obj = Instantiate( carrotPrefab, GameManager.instance.transform );

		//  set pos
		float ang = Random.Range( 0, 360 );
		float radius = Random.Range( radius_range.x, radius_range.y );
		obj.transform.position = transform.position + new Vector3( Mathf.Cos( ang ) * radius, Mathf.Sin( ang ) * radius );

		//  register
		aliveCarrots.Add( obj.transform );	
	}

	Vector2 GetZoneRange( int id )
	{
		Vector2 range = new( 0, zonesRadiuses[id] );

		if ( id - 1 >= 0 )
			range.x = zonesRadiuses[id - 1];

		return range;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		for ( int id = 0; id < zonesRadiuses.Length; id++ )
			Gizmos.DrawWireSphere( transform.position, zonesRadiuses[id] );
	}
}
