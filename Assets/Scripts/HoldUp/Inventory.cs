using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Inventory")]
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private Item defaultItem;

        private Item currentItem;

        private PlayerController playerController;


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

        public void EquipItem(Item itemToEquip)
        {
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(itemToEquip.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController);
        }

        public void DropItem()
        {
            Destroy(currentItem.gameObject);
            currentItem = GameObject.Instantiate(defaultItem.gameObject, transform).GetComponent<Item>();
            currentItem.Initialize(playerController);
        }

        public void UseItem(InputAction.CallbackContext ctx)
        {
            if (currentItem)
            {
                currentItem.OnUse(ctx);
            }
        }

        public void AimItemJoystick(InputAction.CallbackContext ctx)
        {
            if (currentItem)
            {
                currentItem.OnAimJoystick(ctx);
            }
        }

        public void AimItemMouse(InputAction.CallbackContext ctx)
        {
            if (currentItem)
            {
                currentItem.OnAimMouse(ctx);
            }
        }
    }
}
