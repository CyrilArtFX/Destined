using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Tilemaps
{
    [CreateAssetMenu(fileName = "Tilemap Data", menuName = "Core/Tilemap Data")]
    public class TilemapData : ScriptableObject
    {
        public TileBase[] destructiblesTiles;
    }
}
