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

        void Update()
        {
            if(useActionTriggered)
            {
                if(shootTimer <= 0.0f)
                {
                    print("shoot in this direction : " + playerController.LastPerformedDirection);
                    shootTimer = timeBetweenShoots;
                }

            }

            if(shootTimer > 0.0f)
            {
                shootTimer -= Time.deltaTime;
            }
        }
    }
}
