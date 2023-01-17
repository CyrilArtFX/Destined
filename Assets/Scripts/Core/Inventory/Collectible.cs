using System.Collections.Generic;
using UnityEngine;

namespace Core.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class Collectible : MonoBehaviour
    {
        [SerializeField]
        private float collectDelay = 0.5f;

        [SerializeField]
        private new SpriteRenderer renderer;

        private Dictionary<Inventory, float> lastCollectTimes = new();

        private new Collider2D collider;

        void Awake()
        {
            collider = GetComponent<Collider2D>();
        }

        public bool Collect(Inventory inventory)
        {
            if (!CanCollect(inventory)) return false;

            lastCollectTimes[inventory] = Time.time + collectDelay;
            collider.enabled = false;

            OnCollect(inventory);

            return true;
        }

        public void Drop(Inventory inventory)
        {
            lastCollectTimes[inventory] = Time.time + collectDelay;
            collider.enabled = true;

            OnDrop(inventory);
        }

        public virtual bool CanCollect(Inventory inventory)
        {
            if (lastCollectTimes.ContainsKey(inventory))
                return lastCollectTimes[inventory] <= Time.time;

            return true;
        }
        public virtual void OnCollect(Inventory inventory) { }
        public virtual void OnDrop(Inventory inventory) { }
    }
}