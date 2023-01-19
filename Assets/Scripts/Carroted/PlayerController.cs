using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.ComponentModel;

namespace Carroted
{
    [AddComponentMenu( "Scripts/Carroted Player Controller" )]
    public class PlayerController : Core.Players.PlayerController
    {
        public Inventory Inventory => inventory;

        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        private float maxSpeedReduction = .9f;
        [SerializeField]
        private float speedReductionPerItem = 0.15f;

        [SerializeField]
        private ParticleSystem stunParticles;

        [SerializeField]
        private Transform throwStart;

        [SerializeField]
        private GameObject carrotProjectile;

        [Header("Stun Immunity")]

        [SerializeField]
        private float stunImmuneTime;

        [SerializeField]
        private AnimationCurve stunImmuneAnimation;

        public bool IsStun => stun > 0.0f;
        public bool IsStunImmune => stunImmune;

        private bool throwing = false;
        private float stun = 0.0f;
        private bool stunImmune;
        private bool siaHide;
        private float siaHideTime, siaMaxHideTime;


        //  drop action (removed because useless)
        /*
        public void OnDrop(InputAction.CallbackContext ctx)
        {
            if (insideSafeZone) return;
            if (!ctx.action.triggered) return;

            player.Inventory.DropLastItem();
        }*/

        public void OnThrowController(InputAction.CallbackContext ctx)
        {
            if (inventory.Items.Count == 0) return;
            if (InsideSafeZone) return;

            Vector2 throw_direction = ctx.action.ReadValue<Vector2>();

            if (throw_direction.magnitude >= 1.0f)
            {
                if (!throwing)
                {
                    throwing = true;
                    throw_direction.Normalize();

                    SpawnProjectile(throw_direction);
                }
            }
            else if (throw_direction.magnitude <= 0.3f)
            {
                throwing = false;
            }
        }

        public void OnThrowMouse(InputAction.CallbackContext ctx)
        {
            if (inventory.Items.Count == 0) return;
            if (InsideSafeZone) return;

            if (!ctx.action.triggered) return;

            Vector2 mouse_pos = Mouse.current.position.ReadValue();
            mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            Vector2 throw_direction = mouse_pos - (Vector2)transform.position;
            throw_direction.Normalize();

            SpawnProjectile(throw_direction);
        }

        private void SpawnProjectile(Vector2 direction)
        {
            //  get item to consume
            Carrot carrot = null;
            foreach (Carrot item in inventory.Items)
            {
                if (!item.CanBeThrown) continue;

                carrot = item;

                break;
            }

            if (carrot == null) return;

            GameObject projectile = GameObject.Instantiate(carrotProjectile, GameManager.instance.transform);
            projectile.transform.position = throwStart.position;
            projectile.GetComponent<CarrotProjectile>().Initialize(direction, gameObject);

            inventory.RemoveItem(carrot);
            inventory.UpdateItemsPositions();
        }

        public void Stun(float stunTime)
        {
            if (stunImmune || InsideSafeZone || InCinematic) return;

            //  set time
            stun = stunTime;

            //  animation
            animator.SetBool("IsStunned", true);

            //  play stun particles
            var particleMain = stunParticles.main;
            particleMain.startLifetime = stunTime;
            stunParticles.Play();

            //  handle immunity
            StartCoroutine(StunImmunity());

            //  drop all items
            inventory.DropItems();
        }

        void Update()
        {
            if (stunImmune && siaHideTime > 0.0f)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(color.a, siaHide ? 1.0f : 0.0f, (siaMaxHideTime - siaHideTime) / siaMaxHideTime);
                renderer.color = color;
                siaHideTime -= Time.deltaTime;
            }

            //  stun
            if (stun > 0.0f)
            {
                if ((stun -= Time.deltaTime) <= 0.0f)
                {
                    animator.SetBool("IsStunned", false);
                }
            }
            //  move
            else if (!InCinematic)
            {
                float speed = moveSpeed - moveSpeed * Mathf.Min(maxSpeedReduction, inventory.ItemsCount * speedReductionPerItem);
                mover.Move(Direction, speed / moveSpeed, moveSpeed / speed);
            }
        }

        private IEnumerator StunImmunity()
        {
            stunImmune = true;

            float timeImmuned = 0.0f;
            while (timeImmuned < stunImmuneTime)
            {
                siaHide = !siaHide;
                float transitionTime = stunImmuneAnimation.Evaluate(timeImmuned / stunImmuneTime);
                if (transitionTime + timeImmuned > stunImmuneTime)
                {
                    transitionTime = stunImmuneTime - timeImmuned;
                }

                siaHideTime = siaMaxHideTime = transitionTime;
                //print(transitionTime);
                timeImmuned += transitionTime;
                yield return new WaitForSeconds(transitionTime);
            }
            siaHide = false;

            Color color = renderer.color;
            color.a = 1.0f;
            renderer.color = color;

            stunImmune = false;
        }

        public override void ClearEffects()
        {
            base.ClearEffects();

            StopAllCoroutines();
            stunParticles.Stop();
            stunParticles.Clear();
            stun = 0.0f;
            stunImmune = false;
            Color color = renderer.color;
            color.a = 1.0f;
            renderer.color = color;
            animator.SetBool("IsStunned", false);
        }

        void OnDestroy()
        {
            //  drop items
            if(inventory)
            {
                for (int i = 0; i <= inventory.ItemsCount; i++)
                    inventory.DropLastItem();
            }
        }
    }
}
