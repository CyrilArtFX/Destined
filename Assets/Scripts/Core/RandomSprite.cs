using UnityEngine;

namespace Core
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class RandomSprite : MonoBehaviour
	{
		[SerializeField]
		private Sprite[] sprites;

		private new SpriteRenderer renderer;

		void Awake()
		{
			if (sprites.Length > 0)
			{
				renderer = GetComponent<SpriteRenderer>();
				renderer.sprite = sprites[Random.Range(0, sprites.Length)]; 
			}

			Destroy(this);
		}
	}
}