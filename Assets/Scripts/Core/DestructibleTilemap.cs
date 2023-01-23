using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Tilemaps
{
    public class DestructibleTilemap : MonoBehaviour
    {
        private Tilemap tilemap;


        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }


        public void DestroyTilesInRadius(Vector2 center, float radius)
        {
            int minSearchX = Mathf.FloorToInt(center.x - radius);
            int maxSearchX = Mathf.CeilToInt(center.x + radius);
            int minSearchY = Mathf.FloorToInt(center.y - radius);
            int maxSearchY = Mathf.CeilToInt(center.y + radius);

            for(int x = minSearchX; x <= maxSearchX; x++)
            {
                for(int y = minSearchY; y <= maxSearchY; y++)
                {
                    if(Vector2.Distance(center, new Vector2(x, y)) <= radius)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, 0);
                        tilemap.SetTile(tilePos, null);
                    }
                }
            }
        }
    }
}
