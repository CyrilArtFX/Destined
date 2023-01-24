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

        public Vector2 LastPerformedDirection { get; private set; }
        public Vector2 AimDirection { get; private set; }


        private bool mousePressed;
        private float onWaitForMouseReset;

        void Start()
        {
            LastPerformedDirection = Vector2.right;

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
                    mousePressed = true;
                }
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                inventory.UseItemReleased();
                mousePressed = false;
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
            AimDirection = ctx.ReadValue<Vector2>().normalized;
        }

        public void OnMoveMouse(InputAction.CallbackContext ctx)
        {
            Vector2 mouse_pos = Mouse.current.position.ReadValue();
            mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            AimDirection = mouse_pos - (Vector2)transform.position;
            AimDirection.Normalize();

            onWaitForMouseReset = 1.5f;
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
                LastPerformedDirection = Direction.normalized;
            }


            //  reset mouse aim direction if mouse don't move
            if (onWaitForMouseReset > 0.0f && !mousePressed)
            {
                onWaitForMouseReset -= Time.deltaTime;
                if (onWaitForMouseReset <= 0.0f)
                {
                    AimDirection = Vector2.zero;
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
