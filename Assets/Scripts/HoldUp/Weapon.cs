using UnityEngine;

namespace HoldUp
{
    public class Weapon : Item
    {
        public Vector2 Direction => playerController.AimDirection != Vector2.zero ? playerController.AimDirection : playerController.LastPerformedDirection;

        [SerializeField, Tooltip("The number of shot per seconds of this weapon (can be a decimal number)")]
        private float fireRate;
        [SerializeField]
        private bool automaticWeapon;
        [SerializeField, Tooltip("The loss of precision over time when shooting continuously (only works if automaticWeapon is at true)")]
        private AnimationCurve recoilCurve;
        [SerializeField]
        private float timeForMaxRecoil;
        [SerializeField]
        private float recoilPower;

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
        private float recoilTimer;


        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            timeBetweenShoots = 1.0f / fireRate;
        }

        public override void OnUsePressed()
        {
            if (automaticWeapon)
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
            if (automaticWeapon)
            {
                useActionTriggered = false;
            }
        }

        void Update()
        {
            if (automaticWeapon && useActionTriggered)
            {
                if (shootTimer <= 0.0f)
                {
                    float delta = recoilCurve.Evaluate(recoilTimer / timeForMaxRecoil);
                    delta *= recoilPower;
                    delta = Random.Range(-delta, delta);

                    float weaponRotation = transform.rotation.eulerAngles.z;
                    weaponRotation += delta;
                    weaponRotation *= Mathf.Deg2Rad;

                    Vector2 recoiledDirection = new Vector2(Mathf.Cos(weaponRotation), Mathf.Sin(weaponRotation));
                    recoiledDirection.Normalize();

                    Bullet bulletObject = GameObject.Instantiate(bullet.gameObject, GameManager.instance.transform).GetComponent<Bullet>();
                    bulletObject.transform.position = bulletSpawnPos.position;
                    bulletObject.Initialize(bulletSpeed, recoiledDirection, bulletRange, playerController.GetComponent<Collider2D>());

                    shootTimer = timeBetweenShoots;
                }
                recoilTimer = Mathf.Min(recoilTimer + Time.deltaTime, timeForMaxRecoil);
            }
            else
            {
                if (recoilTimer > 0.0f)
                {
                    recoilTimer -= Time.deltaTime * 4.0f;
                }
            }

            if (shootTimer > 0.0f)
            {
                shootTimer -= Time.deltaTime;
            }

            


            Rotate(Direction);


            if (playerController.AimDirection != Vector2.zero)
            {
                redLine.enabled = true;
                RaycastHit2D hitResults = Physics2D.Raycast(bulletSpawnPos.position, bulletSpawnPos.right, redLineMaxDistance, layersBlockingRedLine);
                if (hitResults)
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
