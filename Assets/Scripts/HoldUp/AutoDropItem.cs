using UnityEngine;

namespace HoldUp
{
    public class AutoDropItem : Item
    {
        [SerializeField]
        private GameObject droppedObject;

        public GameObject AutoDrop()
        {
            if (!isHandMode) return null;

            if(inventory)
            {
                inventory.DestroyItemInHand();
            }

            return droppedObject;
        }
    }
}
