using UnityEngine;
using Utility;
using Core.Tilemaps;
using System.Collections;

namespace HoldUp
{
    public class ThrowingObject : MonoBehaviour
    {
        [SerializeField]
        private LayerMask destructibleTilemapLayer;

        [SerializeField]
        private ParticleSystem particles;

        private AnimationCurve powerCurve;
        private float timeForMinimalPower;
        private float radius, damages;

        private CircleCollider2D cc;
        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private Vector2 initialVelocity;
        private float initialVelocitySpeed;
        private float timer;

        void Awake()
        {
            cc = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        public void Initialize(float throwingPower, Vector2 throwingDirection, AnimationCurve throwingPowerCurve,
            float throwingTimeForMinimalPower, Collider2D ownerCollider,
            float explosionRadius, float explosionDamages)
        {
            powerCurve = throwingPowerCurve;
            timeForMinimalPower = throwingTimeForMinimalPower;
            radius = explosionRadius;
            damages = explosionDamages;

            Physics2D.IgnoreCollision(cc, ownerCollider);

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.radius = radius * 0.8f;
            ParticleSystem.EmissionModule emission = particles.emission;
            ParticleSystem.Burst burst = emission.GetBurst(0);
            burst.count = radius;
            emission.SetBurst(0, burst);

            throwingDirection.Normalize();

            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(throwingDirection.y, throwingDirection.x) * Mathf.Rad2Deg));

            rb.AddForce(throwingDirection * throwingPower);
        }

        public void Explode()
        {
            Collider2D[] objectsToExplode = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D col in objectsToExplode)
            {
                if (col.gameObject == gameObject) continue;

                if (LayerMaskUtils.HasLayer(destructibleTilemapLayer, col.gameObject.layer))
                {
                    col.GetComponent<DestructibleTilemap>().DestroyTilesInRadius(transform.position, radius);
                    continue;
                }

                if (col.gameObject.TryGetComponent<Damageable>(out Damageable damageableObject))
                {
                    damageableObject.DealDamages(damages);
                }
            }

            StartCoroutine(DestroyObject());
        }

        void Update()
        {
            if (initialVelocity == Vector2.zero && rb.velocity != Vector2.zero)
            {
                initialVelocity = rb.velocity.normalized;
                initialVelocitySpeed = rb.velocity.magnitude;
            }

            timer = Mathf.Min(timer + Time.deltaTime, timeForMinimalPower);

            rb.velocity = rb.velocity.normalized * initialVelocitySpeed * powerCurve.Evaluate(timer / timeForMinimalPower);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            timer = Mathf.Min(timer + 0.5f, timeForMinimalPower);
        }

        private IEnumerator DestroyObject()
        {
            particles.Play();

            rb.velocity = Vector2.zero;
            cc.enabled = false;
            sr.enabled = false;

            yield return new WaitForSeconds(particles.main.duration + particles.main.startLifetime.constant);

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
