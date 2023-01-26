using System.Linq;
using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Inventory")]
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private Item defaultItem;

        [SerializeField]
        private float radius;

        private Item currentItem;

        private PlayerController playerController;

        private Item itemInRange;


        public void EnableInventory(PlayerController controller)
        {
            playerController = controller;
            EquipItem(defaultItem, false);
        }

        public void DisableInventory()
        {
            if (currentItem)
            {
                Destroy(currentItem.gameObject);
            }
        }

        private void EquipItem(Item itemToEquip, bool canDrop)
        {
            if (currentItem)
            {
                if (!currentItem.IsDefaultItem && canDrop)
                {
                    currentItem.Drop();
                }
                Destroy(currentItem.gameObject);
            }
            currentItem = GameObject.Instantiate(itemToEquip.gameObject, transform).GetComponent<Item>();
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.Initialize(playerController.gameObject, this);
            if (itemToEquip != defaultItem)
            {
                Destroy(itemToEquip.gameObject);
            }
        }

        public void UseItemPressed()
        {
            if (currentItem)
            {
                currentItem.OnUsePressed();
            }
        }

        public void UseItemReleased()
        {
            if (currentItem)
            {
                currentItem.OnUseReleased();
            }
        }

        public void EquipAndDrop(bool forceDrop)
        {
            if (forceDrop)
            {
                if (currentItem && !currentItem.IsDefaultItem)
                {
                    EquipItem(defaultItem, true);
                }
                return;
            }

            itemInRange = null;

            Physics2D.queriesHitTriggers = true;
            Collider2D[] objectDetected = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D collider in objectDetected)
            {
                if (collider.TryGetComponent(out Item itemOnGround))
                {
                    if (!itemOnGround.IsOnGround()) continue;
                    itemInRange = itemOnGround;
                    break;
                }
            }

            if (itemInRange)
            {
                if (currentItem && !currentItem.IsDefaultItem)
                {
                    if (currentItem.ItemID == itemInRange.ItemID)
                    {
                        EquipItem(defaultItem, true);
                    }
                    else
                    {
                        EquipItem(itemInRange, true);
                    }
                }
                else
                {
                    EquipItem(itemInRange, true);
                }
            }
            else
            {
                if (currentItem && !currentItem.IsDefaultItem)
                {
                    EquipItem(defaultItem, true);
                }
            }
            Physics2D.queriesHitTriggers = false;
        }

        public void DestroyItemInHand()
        {
            Destroy(currentItem.gameObject);
            EquipItem(defaultItem, false);
        }

        public Item GetItemInHand()
        {
            return currentItem;
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
