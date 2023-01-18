using System.Collections;
using UnityEngine;

namespace Utility
{
	public static class Physics2DUtils
	{
		public static RaycastHit2D RaycastWithoutTrigger( Vector3 origin, Vector3 dir, float dist, LayerMask mask )
		{
			Physics2D.queriesHitTriggers = false;

			RaycastHit2D hit = Physics2D.Raycast( origin, dir, dist, mask );
			
			Physics2D.queriesHitTriggers = true;
			return hit;
		}
	}
}