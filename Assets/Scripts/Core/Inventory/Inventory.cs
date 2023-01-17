using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Core.Items
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Inventory : MonoBehaviour
    {
        public Transform Owner => owner;
        public ReadOnlyCollection<Collectible> Items => items.AsReadOnly();
        public int ItemsCount => items.Count;
        public int MaxCount => maxCount;

        [SerializeField]
        private int maxCount = 5;
        [SerializeField]
        private float itemOffset = 0.25f;
        [SerializeField]
        private float dropRadius = 0.25f;
        [SerializeField]
        private Transform owner;

        public List<Collectible> items = new();

        public bool AddItem(Collectible item)
        {
            if (items.Count + 1 > maxCount)
                return false;
            if (items.Contains(item))
                return false;
            if (!item.Collect(this))
                return false;

            ClearNulls();

            //  add to container
            items.Add(item);

            //  parent
            item.transform.SetParent(transform);
            UpdateItemPosition(item);

            return true;
        }

        public void UpdateItemPosition(Collectible item)
        {
            item.transform.localPosition = new Vector3(0.0f, itemOffset * items.IndexOf(item));
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }

        public void UpdateItemsPositions()
        {
            ClearNulls();

            foreach (Collectible item in items)
                UpdateItemPosition(item);
        }

        public void DropItem(Collectible item)
        {
            if (!items.Contains(item))
                return;

            //  remove from container
            items.Remove(item);
            if (item == null)
                return;

            //  drop event
            item.Drop(this);

            //  un-parent
            item.transform.SetParent(GameManager.instance.transform);

            //  set pos
            float ang = Random.Range(0.0f, 360.0f);
            item.transform.position = transform.position + new Vector3(Mathf.Cos(ang) * dropRadius, Mathf.Sin(ang) * dropRadius);
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }

        public void DropLastItem()
        {
            if (items.Count <= 0)
                return;

            DropItem(items[items.Count - 1]);
        }

        public void DropItems()
        {
            ClearNulls();

            //  drop all
            while (ItemsCount > 0)
                DropLastItem();
        }

        public void RemoveItem(Collectible item)
        {
            if (!items.Contains(item))
                return;

            //  remove from container
            items.Remove(item);

            //  destroy item
            Destroy(item.gameObject);
        }

        public void ClearInventory()
        {
            ClearNulls();

            for (int i = items.Count - 1; i >= 0; i--)
            {
                RemoveItem(items[i]);
            }
            items.Clear();
        }

        public void ClearNulls()
        {
            int count = items.RemoveAll(x => !x);
            if (count == 0) return;

            UpdateItemsPositions();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out Collectible item))
                return;

            AddItem(item);
        }
    }
}