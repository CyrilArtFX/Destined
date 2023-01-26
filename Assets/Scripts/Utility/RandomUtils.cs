using UnityEngine;

namespace Utility
{
	public static class RandomUtils
	{
		public static T Element<T>(T[] array)
		{
			int length = array.Length;
			switch (length)
			{
				case 0:
					return default;
				case 1:
					return array[0];
				default:
					return array[Random.Range(0, length)];
			}
		}

		public static Vector2 CenterOffset(Vector2 size)
		{
			float half_x = size.x / 2.0f, half_y = size.y / 2.0f;
			return new(
				Random.Range(-half_x, half_x),
				Random.Range(-half_y, half_y)
			);
		}
	}
}