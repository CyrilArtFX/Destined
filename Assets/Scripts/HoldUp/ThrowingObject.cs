using UnityEngine;
using Core.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Core.AI;

namespace HoldUp
{
    public class ThrowingObject : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles;
        [SerializeField]
        private LayerMask obstaclesLayerMask;

        private AnimationCurve powerCurve;
        private float timeForMinimalPower;
        private float radius, damages;

        private CircleCollider2D cc;
        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private Vector2 initialVelocity;
        private float initialVelocitySpeed;
        private float timer;

        private bool hasExplode = false;

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

            if (ownerCollider)
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
            if (hasExplode) return;
            hasExplode = true;

            Physics2D.queriesHitTriggers = true;
            Collider2D[] objectsToExplode = Physics2D.OverlapCircleAll(transform.position, radius);
            List<Damageable> damageableObjects = new();
            List<GameObject> explosiveObjects = new();
            foreach (Collider2D col in objectsToExplode)
            {
                if (col.gameObject == gameObject) continue;

                if (col.gameObject.TryGetComponent(out DestructibleTilemap tilemap))
                {
                    tilemap.DestroyTilesInRadius(transform.position, radius);
                    continue;
                }

                if (col.gameObject.TryGetComponent(out Damageable damageable))
                {
                    damageableObjects.Add(damageable);
                }

                if (col.gameObject.TryGetComponent(out ThrowingItem throwingItem))
                {
                    explosiveObjects.Add(throwingItem.gameObject);
                }

                if (col.gameObject.TryGetComponent(out ThrowingObject throwingObject))
                {
                    explosiveObjects.Add(throwingObject.gameObject);
                }
            }

            //  AI signal
            AISignalizer.Emit(
                new AISignal()
                {
                    Position = transform.position,
                }
            );

            Physics2D.queriesHitTriggers = false;
            StartCoroutine(TestForDamageableObjects(damageableObjects, explosiveObjects));
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

        private IEnumerator TestForDamageableObjects(List<Damageable> damageableObjects, List<GameObject> explosiveObjects)
        {
            particles.Play();

            rb.velocity = Vector2.zero;
            cc.enabled = false;
            sr.enabled = false;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();


            foreach (Damageable damageableObject in damageableObjects)
            {
                bool objectProtected = false;

                Vector2 dir = damageableObject.transform.position - transform.position;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, obstaclesLayerMask);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == gameObject) continue;

                    if (hit.collider.gameObject != damageableObject.gameObject)
                    {
                        objectProtected = true;
                        break;
                    }
                }
                if (!objectProtected)
                {
                    damageableObject.DealDamages(damages, transform.position);
                }
            }

            yield return new WaitForSeconds(0.3f);

            foreach (GameObject explosive in explosiveObjects)
            {
                if (!explosive) continue;
                bool objectProtected = false;

                Vector2 dir = explosive.transform.position - transform.position;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, obstaclesLayerMask);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == gameObject) continue;

                    if (hit.collider.gameObject != explosive)
                    {
                        objectProtected = true;
                        break;
                    }
                }
                if (!objectProtected)
                {
                    if (explosive.TryGetComponent(out ThrowingItem throwingItem))
                    {
                        throwingItem.ForceInstantExplosion();
                    }

                    if (explosive.TryGetComponent(out ThrowingObject throwingObject))
                    {
                        throwingObject.Explode();
                    }
                }
            }

            StartCoroutine(DestroyObject());
        }

        private IEnumerator DestroyObject()
        {
            yield return new WaitForSeconds(particles.main.duration + particles.main.startLifetime.constant - 0.3f);

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
