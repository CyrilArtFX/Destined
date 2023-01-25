using Core.Astar2D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Tilemaps
{
    public class DestructibleTilemap : MonoBehaviour
    {
        private Tilemap tilemap;

        [SerializeField]
        private TilemapData tilemapData;

        private HashSet<TileBase> destructiblesTiles = new();
        private bool shouldRefreshGrid = false;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            if(tilemapData)
            {
                foreach(TileBase tile in tilemapData.destructiblesTiles)
                {
                    destructiblesTiles.Add(tile);
                }
            }
        }


        public void DestroyTilesInRadius(Vector2 center, float radius)
        {
            int minSearchX = Mathf.FloorToInt(center.x - radius);
            int maxSearchX = Mathf.CeilToInt(center.x + radius);
            int minSearchY = Mathf.FloorToInt(center.y - radius);
            int maxSearchY = Mathf.CeilToInt(center.y + radius);

            bool tilesDestroyed = false;

            for(int x = minSearchX; x <= maxSearchX; x++)
            {
                for(int y = minSearchY; y <= maxSearchY; y++)
                {
                    if(Vector2.Distance(center, new Vector2(x, y)) <= radius)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, 0);
                        TileBase tile = tilemap.GetTile(tilePos);
                        if (tile && destructiblesTiles.Contains(tile))
                        {
                            tilemap.SetTile(tilePos, null);
                            tilesDestroyed = true;
                        }
                    }
                }
            }

            if (tilesDestroyed)
            {
                shouldRefreshGrid = true;
            }
        }

		void LateUpdate()
		{
			if (shouldRefreshGrid)
            {
                Grid2D.Instance.Refresh();
                shouldRefreshGrid = false;
            }
		}
	}
}
