using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Player Controller")]
    public class PlayerController : Core.Players.PlayerController
    {
        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        private Damageable damageable;

        public Inventory Inventory => inventory;

        private Vector2 lastPerformedDirection;
        private Vector2 aimDirection;
        private bool aimWithJoystick;

        void Start()
        {
            lastPerformedDirection = Vector2.right;

            inventory.EnableInventory(this);
        }

        public void OnDebugAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;
        }

        public void OnUseItemAction(InputAction.CallbackContext ctx)
        {
            if (ctx.action.WasPressedThisFrame())
            {
                if (ctx.action.triggered)
                {
                    inventory.UseItemPressed();
                }
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                inventory.UseItemReleased();
            }
        }

        public void OnEquipAndDropAction(InputAction.CallbackContext ctx)
        {
            if (ctx.action.triggered)
            {
                inventory.EquipAndDrop();
            }
        }

        public void OnMoveRightJoystick(InputAction.CallbackContext ctx)
        {
            aimDirection = ctx.ReadValue<Vector2>().normalized;
            aimWithJoystick = true;
        }

        public void OnMoveMouse(InputAction.CallbackContext ctx)
        {
            Vector2 mouse_pos = Mouse.current.position.ReadValue();
            mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            aimDirection = mouse_pos - (Vector2)transform.position;
            aimDirection.Normalize();
            aimWithJoystick = false;
        }

        void Update()
        {
            //  move
            if (!InCinematic)
            {
                mover.Move(Direction);
            }

            if (Direction != Vector2.zero)
            {
                lastPerformedDirection = Direction.normalized;
            }


            Inventory.GetItemInHand().SetDirection(aimDirection == Vector2.zero ? lastPerformedDirection : aimDirection);

            Weapon weaponInHand = (Inventory.GetItemInHand() as Weapon);
            if(weaponInHand)
            {
                if (aimWithJoystick && aimDirection != Vector2.zero)
                {
                    weaponInHand.ShowRedLine();
                }
                else
                {
                    weaponInHand.HideRedLine();
                }
            }
        }

        public override void ClearEffects()
        {
            base.ClearEffects();

            damageable.ResetLife();
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
