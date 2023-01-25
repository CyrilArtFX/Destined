using TMPro;
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
        [SerializeField]
        private Interactable interactable;

        [SerializeField]
        private TextMeshPro interactionDisplayText;

        [SerializeField]
        private float timeForRevive;
        [SerializeField]
        private Lifebar reviveBar;

        public Inventory Inventory => inventory;

        public bool Dead { get; private set; }

        private Vector2 lastPerformedDirection;
        private Vector2 aimDirection;
        private bool aimWithJoystick;
        private float currentlyRevived;

        private Interactable useableInteraction;

        void Start()
        {
            lastPerformedDirection = Vector2.right;
            Dead = false;
            interactable.InteractionPossible = false;

            reviveBar.Initialize(timeForRevive);

            inventory.EnableInventory(this);
        }

        public void OnDebugAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;
        }

        public void OnUseItemAction(InputAction.CallbackContext ctx)
        {
            if (Dead) return;

            if (ctx.action.WasPressedThisFrame())
            {
                if (ctx.action.triggered)
                {
                    if (useableInteraction && useableInteraction.InteractionPossible)
                    {
                        useableInteraction.PressInteraction.Invoke(this);
                        return;
                    }

                    inventory.UseItemPressed();
                }
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                if (useableInteraction && useableInteraction.InteractionPossible)
                {
                    useableInteraction.ReleaseInteraction.Invoke(this);
                    return;
                }
                inventory.UseItemReleased();
            }
        }

        public void OnEquipAndDropAction(InputAction.CallbackContext ctx)
        {
            if (Dead) return;

            if (ctx.action.triggered)
            {
                inventory.EquipAndDrop(false);
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
            if (currentlyRevived > 0.0f)
            {
                currentlyRevived -= Time.deltaTime;
                reviveBar.ChangeLife(Mathf.Min(timeForRevive - currentlyRevived, timeForRevive));
                if (currentlyRevived <= 0.0f)
                {
                    Revive();
                }
            }

            if (Dead)
            {
                mover.Move(Vector2.zero);
                return;
            }

            if (useableInteraction && useableInteraction.InteractionPossible)
            {
                interactionDisplayText.text = useableInteraction.InteractionDisplay;
            }
            else
            {
                interactionDisplayText.text = "";
            }

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
            if (weaponInHand)
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
            interactable.InteractionPossible = false;
            Dead = false;
        }

        public void SetDead()
        {
            Weapon hold_weapon = Inventory.GetItemInHand() as Weapon;
            if (hold_weapon)
            {
                hold_weapon.HideRedLine();
            }

            inventory.UseItemReleased();
            inventory.EquipAndDrop(true);
            interactable.InteractionPossible = true;
            reviveBar.ChangeLife(0.0f);

            if (useableInteraction)
            {
                useableInteraction.ExitZoneInteraction.Invoke(this);
                useableInteraction = null;
                interactionDisplayText.text = "";
            }

            if (!player.IsMenuMode)
            {
                BankManager.instance.PlayerDead();
            }

            Dead = true;
        }

        public void OnPressRevive()
        {
            if (Dead)
            {
                currentlyRevived = timeForRevive;
            }
        }

        public void OnReleaseRevive()
        {
            if (Dead)
            {
                currentlyRevived = 0.0f;
                reviveBar.ChangeLife(0.0f);
            }
        }

        private void Revive()
        {
            Dead = false;
            damageable.ResetLife();
            interactable.InteractionPossible = false;

            if (!player.IsMenuMode)
            {
                BankManager.instance.PlayerRevived();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (Dead) return;

            if (collision.gameObject.TryGetComponent(out Interactable otherInteractable))
            {
                useableInteraction = otherInteractable;
                inventory.UseItemReleased();
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (Dead) return;

            if (collision.gameObject.TryGetComponent(out Interactable otherInteractable))
            {
                if (useableInteraction == otherInteractable)
                {
                    useableInteraction.ExitZoneInteraction.Invoke(this);
                    useableInteraction = null;
                    interactionDisplayText.text = "";
                }
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
