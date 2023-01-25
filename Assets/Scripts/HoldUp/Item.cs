using UnityEngine;

namespace HoldUp
{
    public abstract class Item : MonoBehaviour
    {
        protected Vector2 direction;

        public bool IsDefaultItem;
        public ItemOnGround ItemOnGround;
        public int ItemID;

        protected GameObject owner;
        protected Inventory inventory;

        public virtual void Initialize(GameObject ownerActor, Inventory ownerInventory)
        {
            owner = ownerActor;
            inventory = ownerInventory;
        }

        public virtual void OnUsePressed()
        {
        }

        public virtual void OnUseReleased()
        {
        }

        protected void Rotate(Vector2 rotationDirection)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg));
        }

        public virtual void Drop()
        {
            GameObject droppedItem = GameObject.Instantiate(ItemOnGround.gameObject, GameManager.instance.transform);
            droppedItem.transform.position = owner.transform.position;
        }

        public void SetDirection(Vector2 itemDirection)
        {
            direction = itemDirection;
        }
    }
}
