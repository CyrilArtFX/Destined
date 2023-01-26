using System.Collections.Generic;
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

        private List<Interactable> useableInteractions = new();

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
                    if (useableInteractions.Count > 0)
                    {
                        bool interactionDone = false;
                        foreach (Interactable interaction in useableInteractions)
                        {
                            if (interaction.InteractionPossible)
                            {
                                interaction.PressInteraction.Invoke(this);
                                interactionDone = true;
                            }
                        }
                        if (interactionDone) return;
                    }

                    inventory.UseItemPressed();
                }
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                if (useableInteractions.Count > 0)
                {
                    bool interactionDone = false;
                    foreach (Interactable interaction in useableInteractions)
                    {
                        if (interaction.InteractionPossible)
                        {
                            interaction.ReleaseInteraction.Invoke(this);
                            interactionDone = true;
                        }
                    }
                    if (interactionDone) return;
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

            if (useableInteractions.Count > 0)
                RefreshInteractionText();

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

        public void TryExplodeThrowingItem()
        {
            ThrowingItem item = Inventory.GetItemInHand() as ThrowingItem;
            if(item)
            {
                item.ForceInstantExplosion();
            }
        }

        public override void ClearEffects()
        {
            base.ClearEffects();

            damageable.ResetLife();
            interactable.InteractionPossible = false;
            if(Dead)
            {
                Revive();
            }
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
            currentlyRevived = 0.0f;

            if (useableInteractions.Count > 0)
            {
                foreach (Interactable interaction in useableInteractions)
                {
                    interaction.ExitZoneInteraction.Invoke(this);
                }
                useableInteractions.Clear();
                interactionDisplayText.text = "";
            }

            if (!player.IsMenuMode)
            {
                BankManager.instance.PlayerDead();
            }

            animator.SetBool("IsDead", true);
            Dead = true;
        }

        public void OnPressRevive()
        {
            if (Dead && currentlyRevived == 0.0f)
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
            animator.SetBool("IsDead", false);
            damageable.ResetLife();
            interactable.InteractionPossible = false;
            currentlyRevived = 0.0f;
            reviveBar.ChangeLife(timeForRevive);

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
                if(useableInteractions.Contains(otherInteractable))
                {
                    Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collision, true);
                    return;
                }

                useableInteractions.Add(otherInteractable);
                inventory.UseItemReleased();
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (Dead) return;

            if (collision.gameObject.TryGetComponent(out Interactable otherInteractable))
            {
                if (useableInteractions.Contains(otherInteractable)) return;

                useableInteractions.Add(otherInteractable);
                inventory.UseItemReleased();
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (Dead) return;

            if (collision.gameObject.TryGetComponent(out Interactable otherInteractable))
            {
                if (useableInteractions.Contains(otherInteractable))
                {
                    useableInteractions.Remove(otherInteractable);
                    otherInteractable.ExitZoneInteraction.Invoke(this);
                    RefreshInteractionText();
                }
            }
        }

        private void RefreshInteractionText()
        {
            string str = "";
            if (useableInteractions.Count > 0)
            {
                bool notFirstInteraction = false;
                foreach (Interactable interaction in useableInteractions)
                {
                    if (interaction.InteractionPossible)
                    {
                        if (notFirstInteraction)
                            str += " & ";
                        str += interaction.InteractionDisplay;
                        notFirstInteraction = true;
                    }
                }
            }
            interactionDisplayText.text = str;
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
