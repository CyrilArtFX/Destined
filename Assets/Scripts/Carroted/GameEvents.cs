using UnityEngine;
using UnityEngine.Events;

namespace Carroted
{
    public static class GameEvents
    {
        public static UnityEvent<Transform, Collectible> OnCollect = new();
    }
}
