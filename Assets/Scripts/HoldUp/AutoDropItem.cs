using UnityEngine;

namespace HoldUp
{
    public class AutoDropItem : Item
    {
        [SerializeField]
        private GameObject droppedObject;

        public GameObject AutoDrop()
        {
            playerController.Inventory.DestroyItemInHand();

            return droppedObject;
        }
    }
}
