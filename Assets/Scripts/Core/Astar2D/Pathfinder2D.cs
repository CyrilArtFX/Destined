using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.Astar2D
{
	public class Pathfinder2D : MonoBehaviour
	{
		public List<Node2D> Path { get; private set; }

		void Awake()
		{
			Path = new();
		}

		public bool FindPath( Vector2 target_pos )
		{
			//  get player and target position in grid coords
			Node2D seeker_node = Grid2D.Instance.GetNodeAt( transform.position );
			if (seeker_node == null)
			{
				Debug.LogWarning("Pathfinder2D: seeker node is null");
				return false;
			}

			Node2D target_node = Grid2D.Instance.GetNodeAt( target_pos );
			if (target_node == null)
			{
				Debug.LogWarning("Pathfinder2D: target node is null");
				return false;
			}

			//  setup open & closed sets
			List<Node2D> open_set = new();
			HashSet<Node2D> closed_set = new();
			open_set.Add( seeker_node );

			//  calculates path for pathfinding
			while ( open_set.Count > 0 )
			{
				//  iterates through open-set and finds lowest FCost
				Node2D node = open_set[0];
				for ( int i = 1; i < open_set.Count; i++ )
				{
					if ( open_set[i].FCost <= node.FCost )
					{
						if ( open_set[i].HCost < node.HCost )
							node = open_set[i];
					}
				}

				open_set.Remove( node );
				closed_set.Add( node );

				//  if target found, retrace path
				if ( node == target_node )
				{
					RetracePath( seeker_node, target_node );
					return true;
				}

				//  adds neighbor nodes to openSet
				foreach ( Node2D neighbour in Grid2D.Instance.GetNeighbors( node ) )
				{
					if ( neighbour.IsObstacle || closed_set.Contains( neighbour ) ) continue;

					int new_cost_to_neighbour = node.GCost + GetDistance( node, neighbour );
					if ( new_cost_to_neighbour < neighbour.GCost || !open_set.Contains( neighbour ) )
					{
						neighbour.GCost = new_cost_to_neighbour;
						neighbour.HCost = GetDistance( neighbour, target_node );
						neighbour.parent = node;

						if ( !open_set.Contains( neighbour ) )
							open_set.Add( neighbour );
					}
				}
			}

			Debug.LogWarning("Pathfinder2D: path could not be found");
			return false;
		}

		//  reverses calculated path so first node is closest to seeker
		void RetracePath(Node2D start_node, Node2D end_node)
		{
			Path.Clear();

			Node2D current_node = end_node;
			while ( current_node != start_node )
			{
				Path.Add( current_node );
				current_node = current_node.parent;
			}

			Path.Reverse();
		}

		//  gets distance between 2 nodes for calculating cost
		int GetDistance( Node2D a, Node2D b )
		{
			int dst_x = Mathf.Abs( a.GridX - b.GridX );
			int dst_y = Mathf.Abs( a.GridY - b.GridY );

			if ( dst_x > dst_y )
				return 14 * dst_y + 10 * ( dst_x - dst_y );

			return 14 * dst_x + 10 * ( dst_y - dst_x );
		}
	}
}