using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    public class Weapon : Item
    {
        public Vector2 Direction => playerController.AimDirection != Vector2.zero? playerController.AimDirection : playerController.LastPerformedDirection;

        [SerializeField, Tooltip("The number of shot per seconds of this weapon (can be a decimal number)")]
        private float fireRate;
        [SerializeField]
        private bool automaticWeapon;

        [SerializeField]
        private Bullet bullet;
        [SerializeField]
        private float bulletSpeed, bulletRange;
        [SerializeField]
        private Transform bulletSpawnPos;


        private bool useActionTriggered;
        private float timeBetweenShoots, shootTimer;


        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            timeBetweenShoots = 1.0f / fireRate;
        }

        public override void OnUsePressed()
        {
            useActionTriggered = true;
        }

        public override void OnUseReleased()
        {
            useActionTriggered = false;
        }

        void Update()
        {
            if(useActionTriggered)
            {
                if(shootTimer <= 0.0f)
                {
                    Bullet bulletObject = GameObject.Instantiate(bullet.gameObject, GameManager.instance.transform).GetComponent<Bullet>();
                    bulletObject.Initialize(bulletSpeed, Direction, bulletRange, playerController.GetComponent<Collider2D>());
                    bulletObject.transform.position = bulletSpawnPos.position;

                    shootTimer = timeBetweenShoots;
                }
            }

            if(shootTimer > 0.0f)
            {
                shootTimer -= Time.deltaTime;
            }


            Rotate(Direction);
        }
    }
}
