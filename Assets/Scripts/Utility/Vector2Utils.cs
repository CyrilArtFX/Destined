using System.Collections;
using UnityEngine;

namespace Utility
{
	public static class Vector2Utils
	{
		/// <summary>
		/// Returns the angle (in degrees) from the direction vector
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static float GetDirectionAngle(this Vector2 dir)
		{
			return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		}

		/// <summary>
		/// Returns the euler angles as Vector3 (in degrees) from the direction vector
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static Vector3 GetDirectionAngles(this Vector2 dir)
		{
			return new(0.0f, 0.0f, GetDirectionAngle(dir));
		}
	}
}