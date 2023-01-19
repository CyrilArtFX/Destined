using UnityEngine;
using UnityEngine.InputSystem;
using Core;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Player Controller")]
    public class PlayerController : Core.Players.PlayerController
    {
        [SerializeField]
        private Lifebar lifebar;

        [SerializeField]
        private Inventory inventory;

        public Inventory Inventory => inventory;

        void Start()
        {
            lifebar.Initialize(15.0f);

            inventory.EnableInventory();
        }

        public void OnDebugAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;

            lifebar.LoseLife(1.0f);
        }

        public void OnUseItemAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;

            inventory.UseItem();
        }

        void Update()
        {
            //  move
            if (!InCinematic)
            {
                mover.Move(Direction);
            }
        }

        void OnDestroy()
        {
            if(inventory)
            {
                inventory.DisableInventory();
            }
        }
    }
}
