using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Astar2D
{
	public class Node2D
	{
		public int GCost, HCost;
		public int FCost
		{
			get
			{
				return GCost + HCost;
			}
		}

		public bool IsObstacle;
		public Vector3 Position;

		public int GridX, GridY;
		public Node2D parent;

		public Node2D( bool is_obstacle, Vector3 world_pos, int grid_x, int grid_y )
		{
			IsObstacle = is_obstacle;
			Position = world_pos;
			GridX = grid_x;
			GridY = grid_y;
		}
	}
}