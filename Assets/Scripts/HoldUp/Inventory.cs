using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Inventory")]
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private Item defaultItem;

        private Item currentItem;


        public void EnableInventory()
        {
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
        }

        public void DisableInventory()
        {
            if(currentItem)
            {
                Destroy(currentItem.gameObject);
            }
        }

        public void EquipItem(Item itemToEquip)
        {
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(itemToEquip.gameObject, transform).GetComponent<Item>();
        }

        public void DropItem()
        {
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
        }

        public void UseItem()
        {
            if(currentItem)
            {
                currentItem.OnUse();
            }
        }
    }
}
