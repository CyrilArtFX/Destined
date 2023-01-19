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

        public Vector2 LastPerformedDirection { get; private set; }

        void Start()
        {
            lifebar.Initialize(15.0f);

            LastPerformedDirection = Vector2.right;

            inventory.EnableInventory(this);
        }

        public void OnDebugAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;

            lifebar.LoseLife(1.0f);
        }

        public void OnUseItemAction(InputAction.CallbackContext ctx)
        {
            inventory.UseItem(ctx);
        }

        public void OnMoveRightJoystick(InputAction.CallbackContext ctx)
        {
            inventory.AimItemJoystick(ctx);
        }

        public void OnMoveMouse(InputAction.CallbackContext ctx)
        {
            inventory.AimItemMouse(ctx);
        }

        void Update()
        {
            //  move
            if (!InCinematic)
            {
                mover.Move(Direction);
            }

            if(Direction != Vector2.zero)
            {
                LastPerformedDirection = Direction.normalized;
            }
        }

        void OnDestroy()
        {
            if (inventory)
            {
                inventory.DisableInventory();
            }
        }
    }
}
