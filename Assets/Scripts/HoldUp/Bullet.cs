using UnityEngine;

namespace HoldUp
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody2D rb;
        private CircleCollider2D cc;

        private float speed;
        private Vector2 direction;
        private float range;
        private float damages;

        private Vector2 spawnPosition;

        private int numberOfHits = 0;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            cc = GetComponent<CircleCollider2D>();
        }

        public void Initialize(float bulletSpeed, Vector2 bulletDirection, float bulletRange, float bulletDamages, Collider2D ownerCollider)
        {
            speed = bulletSpeed;
            direction = bulletDirection;
            range = bulletRange;
            damages = bulletDamages;

            spawnPosition = transform.position;
            Physics2D.IgnoreCollision(cc, ownerCollider);

            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        }

        void Update()
        {
            rb.MovePosition(rb.position + direction * speed);

            if(Vector2.Distance(spawnPosition, transform.position) > range)
            {
                DestroyBullet();
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (numberOfHits > 0) return; //  avoid hitting multiple damageables

            if(collision.gameObject.TryGetComponent(out Damageable damageable))
            {
                damageable.DealDamages(damages, transform.position);
            }

            numberOfHits++;
            DestroyBullet();
        }

        private void DestroyBullet()
        {
            Destroy(gameObject);
        }
    }
}
