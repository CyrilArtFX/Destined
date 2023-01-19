using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Inventory")]
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private Item defaultItem;

        [SerializeField]
        private LayerMask itemOnGroundMask;

        [SerializeField]
        private float radius;

        private Item currentItem;

        private PlayerController playerController;

        private ItemOnGround itemInRange;


        public void EnableInventory(PlayerController controller)
        {
            playerController = controller;
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController);
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
            currentItem.Initialize(playerController);
            Destroy(itemToEquip.gameObject);
        }

        private void DropItem()
        {
            currentItem.Drop();
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController);
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

        public void EquipAndDrop()
        {
            Physics2D.queriesHitTriggers = true;
            Collider2D itemOnGround = Physics2D.OverlapCircle(transform.position, radius, itemOnGroundMask);
            if(itemOnGround)
            {
                itemInRange = itemOnGround.GetComponent<ItemOnGround>();
            }
            else
            {
                itemInRange = null;
            }

            if(itemInRange)
            {
                EquipItem(itemInRange);
            }
            else
            {
                if(!currentItem.IsDefaultItem)
                {
                    DropItem();
                }
            }
            Physics2D.queriesHitTriggers = false;
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
