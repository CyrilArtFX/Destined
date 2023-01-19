using UnityEngine;

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

        [SerializeField]
        private LineRenderer redLine;
        [SerializeField]
        private float redLineMaxDistance;
        [SerializeField]
        private LayerMask layersBlockingRedLine;


        private bool useActionTriggered;
        private float timeBetweenShoots, shootTimer;


        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            timeBetweenShoots = 1.0f / fireRate;
        }

        public override void OnUsePressed()
        {
            if(automaticWeapon)
            {
                useActionTriggered = true;
            }
            else
            {
                if (shootTimer <= 0.0f)
                {
                    Bullet bulletObject = GameObject.Instantiate(bullet.gameObject, GameManager.instance.transform).GetComponent<Bullet>();
                    bulletObject.transform.position = bulletSpawnPos.position;
                    bulletObject.Initialize(bulletSpeed, Direction.normalized, bulletRange, playerController.GetComponent<Collider2D>());

                    shootTimer = timeBetweenShoots;
                }
            }
        }

        public override void OnUseReleased()
        {
            if(automaticWeapon)
            {
                useActionTriggered = false;
            }
        }

        void Update()
        {
            if(automaticWeapon && useActionTriggered)
            {
                if(shootTimer <= 0.0f)
                {
                    Bullet bulletObject = GameObject.Instantiate(bullet.gameObject, GameManager.instance.transform).GetComponent<Bullet>();
                    bulletObject.transform.position = bulletSpawnPos.position;
                    bulletObject.Initialize(bulletSpeed, Direction.normalized, bulletRange, playerController.GetComponent<Collider2D>());

                    shootTimer = timeBetweenShoots;
                }
            }

            if(shootTimer > 0.0f)
            {
                shootTimer -= Time.deltaTime;
            }


            Rotate(Direction);


            if(playerController.AimDirection != Vector2.zero)
            {
                redLine.enabled = true;
                RaycastHit2D hitResults = Physics2D.Raycast(bulletSpawnPos.position, bulletSpawnPos.right, redLineMaxDistance, layersBlockingRedLine);
                if(hitResults)
                {
                    redLine.SetPosition(1, new Vector3(hitResults.distance, 0.0f, 0.0f));
                }
                else
                {
                    redLine.SetPosition(1, new Vector3(redLineMaxDistance, 0.0f, 0.0f));
                }
            }
            else
            {
                redLine.enabled = false;
            }
        }
    }
}
