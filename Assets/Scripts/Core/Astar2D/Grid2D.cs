using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Astar2D
{
	public class Grid2D : MonoBehaviour
	{
		public static Grid2D Instance { get; private set; }

		public Vector2Int GridWorldSize;
		public float NodeRadius;
		public Node2D[,] Grid;
		public Tilemap ObstacleMap;
		public bool CanUseDiagonals = false;

		Vector2Int gridSize;
		Vector3 worldBottomLeft;
		float nodeDiameter;

		void Awake()
		{
			Instance = this;

			nodeDiameter = NodeRadius * 2;

			gridSize.x = Mathf.RoundToInt( GridWorldSize.x / nodeDiameter );
			gridSize.y = Mathf.RoundToInt( GridWorldSize.y / nodeDiameter );

			CreateGrid();
		}

		void CreateGrid()
		{
			Grid = new Node2D[gridSize.x, gridSize.y];
			worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2;

			for ( int x = 0; x < gridSize.x; x++ )
			{
				for ( int y = 0; y < gridSize.y; y++ )
				{
					Vector3 world_pos = ToWorldPos( x, y );
					Grid[x, y] = new( ObstacleMap.HasTile( ObstacleMap.WorldToCell( world_pos ) ), world_pos, x, y );
					//Grid[x, y] = new( Physics2D.Raycast( world_pos, Vector2.up ), world_pos, x, y );
				}
			}

			print( $"Grid2D: created {Grid.Length} nodes!" );
		}

		public List<Node2D> GetNeighbors( Node2D node )
		{
			List<Node2D> neighbors = new();

			void TryAdd( int offset_x, int offset_y )
			{
				if ( node.GridX + offset_x >= 0 && node.GridX + offset_x < gridSize.x
				  && node.GridY + offset_y >= 0 && node.GridY + offset_y < gridSize.y )
					neighbors.Add( Grid[node.GridX + offset_x, node.GridY + offset_y] );
			}

			//  add cardinals
			TryAdd( 0, 1 );
			TryAdd( 0, -1 );
			TryAdd( 1, 0 );
			TryAdd( -1, 0 );

			//  add diagonals
			if ( CanUseDiagonals )
			{
				TryAdd( 1, 1 );
				TryAdd( -1, 1 );
				TryAdd( 1, -1 );
				TryAdd( -1, -1 );
			}

			return neighbors;
		}

		public Vector3 ToWorldPos( int x, int y )
		{
			return worldBottomLeft + new Vector3( x * nodeDiameter + NodeRadius, y * nodeDiameter + NodeRadius );
		}

		public Vector2Int ToCellPos( Vector2 world_pos )
		{
			return new(
				Mathf.RoundToInt( ( world_pos.x - worldBottomLeft.x - NodeRadius ) / nodeDiameter ),
				Mathf.RoundToInt( ( world_pos.y - worldBottomLeft.y - NodeRadius ) / nodeDiameter )
			);
		}

		public Node2D GetNodeAt( Vector2 world_pos )
		{
			Vector2Int cell = ToCellPos( world_pos );

			//  check if in-bounds
			if ( cell.x < 0 || cell.x > gridSize.x || cell.y < 0 || cell.y > gridSize.y )
				return null;

			return Grid[cell.x, cell.y];
		}

		//  draws visual representation of grid
		void OnDrawGizmos()
		{
			//  draw region
			Gizmos.DrawWireCube( transform.position, new Vector3( GridWorldSize.x, GridWorldSize.y, 1 ) );

			//  draw nodes
			if ( Grid != null )
			{
				foreach ( Node2D n in Grid )
				{
					Color color = n.IsObstacle ? Color.red : Color.white;
					color.a = 0.33f;
					Gizmos.color = color;

					Gizmos.DrawWireCube( n.Position, Vector3.one * NodeRadius );
				}
			}
		}
	}
}
