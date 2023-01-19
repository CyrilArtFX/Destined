using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    public class Weapon : Item
    {
        [SerializeField, Tooltip("The number of shot per seconds of this weapon (can be a decimal number)")]
        private float fireRate;


        private bool useActionTriggered;
        private float timeBetweenShoots, shootTimer;

        private Vector2 aimDirection;


        private float onWaitForMouseReset;


        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            timeBetweenShoots = 1.0f / fireRate;
        }

        public override void OnUse(InputAction.CallbackContext ctx)
        {
            if (ctx.action.WasPressedThisFrame())
            {
                useActionTriggered = true;
            }
            if (ctx.action.WasReleasedThisFrame())
            {
                useActionTriggered = false;
            }
        }

        public override void OnAimJoystick(InputAction.CallbackContext ctx)
        {
            aimDirection = ctx.ReadValue<Vector2>().normalized;
        }

        public override void OnAimMouse(InputAction.CallbackContext ctx)
        {
            Vector2 mouse_pos = Mouse.current.position.ReadValue();
            mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            aimDirection = mouse_pos - (Vector2)transform.position;
            aimDirection.Normalize();

            onWaitForMouseReset = 0.5f;
        }

        void Update()
        {
            if(useActionTriggered)
            {
                if(shootTimer <= 0.0f)
                {
                    print("shoot");
                    shootTimer = timeBetweenShoots;
                }

            }

            if(shootTimer > 0.0f)
            {
                shootTimer -= Time.deltaTime;
            }


            if(onWaitForMouseReset > 0.0f)
            {
                onWaitForMouseReset -= Time.deltaTime;
                if(onWaitForMouseReset <= 0.0f)
                {
                    aimDirection = Vector2.zero;
                }
            }


            Rotate(aimDirection != Vector2.zero ? aimDirection : playerController.LastPerformedDirection);
        }
    }
}
