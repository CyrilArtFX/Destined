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

        private ItemOnGround itemInRange;


        public void EnableInventory(PlayerController controller)
        {
            playerController = controller;
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController.gameObject, this);
        }

        public void DisableInventory()
        {
            if (currentItem)
            {
                Destroy(currentItem.gameObject);
            }
        }

        private void EquipItem(ItemOnGround itemToEquip)
        {
            if(!currentItem.IsDefaultItem)
            {
                currentItem.Drop();
            }
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(itemToEquip.Item.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController.gameObject, this);
            Destroy(itemToEquip.gameObject);
        }

        private void DropItem()
        {
            currentItem.Drop();
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController.gameObject, this);
        }

        public void UseItemPressed()
        {
            if(currentItem)
            {
                currentItem.OnUsePressed();
            }
        }

        public void UseItemReleased()
        {
            if(currentItem)
            {
                currentItem.OnUseReleased();
            }
        }

        public void EquipAndDrop(bool forceDrop)
        {
            if(forceDrop)
            {
                if (currentItem && !currentItem.IsDefaultItem)
                {
                    DropItem();
                }
                return;
            }

            itemInRange = null;

            Physics2D.queriesHitTriggers = true;
            Collider2D[] objectDetected = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach(Collider2D collider in objectDetected)
            {
                if(collider.TryGetComponent(out ItemOnGround itemOnGround))
                {
                    itemInRange = itemOnGround;
                    break;
                }
            }

            if(itemInRange)
            {
                if(currentItem && !currentItem.IsDefaultItem)
                {
                    if(currentItem.ItemID == itemInRange.Item.ItemID)
                    {
                        DropItem();
                    }
                    else
                    {
                        EquipItem(itemInRange);
                    }
                }
                else
                {
                    EquipItem(itemInRange);
                }
            }
            else
            {
                if(currentItem && !currentItem.IsDefaultItem)
                {
                    DropItem();
                }
            }
            Physics2D.queriesHitTriggers = false;
        }

        public void DestroyItemInHand()
        {
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController.gameObject, this);
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
