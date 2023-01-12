using System.Collections;
using UnityEngine;

public class CarrotProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float range;

    [SerializeField]
    private float stunTime;

    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private GameObject carrot;

    [SerializeField]
    private ParticleSystem impactParticles;


    private Vector2 direction;
    private Vector3 startPosition;
    private GameObject owner;

    [Header("Colliders")]
    [SerializeField]
    CircleCollider2D triggerCC;
    [SerializeField]
    CircleCollider2D collisionCC;

    private SpriteRenderer sr;
    private TrailRenderer trail;
    private Rigidbody2D rb;

    private bool destroyed = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 newDirection, GameObject newOwner)
    {
        direction = newDirection;
        startPosition = transform.position;
        owner = newOwner;

        transform.right = newDirection;

        Physics2D.IgnoreCollision(collisionCC, newOwner.GetComponent<Collider2D>(), true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;

        if (collision.gameObject == owner) return;

        if (collision.gameObject.GetComponent<PlayerController>().IsStunImmune)
        {
            Physics2D.IgnoreCollision(collisionCC, collision, true);
            return;
        }

        collision.gameObject.GetComponent<PlayerController>().Stun(stunTime);

        impactParticles.Play();
        StartCoroutine(DestroyProjectile(false));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(DestroyProjectile(true));
    }

    private void Update()
    {
        if (destroyed) return;

        if (direction != Vector2.zero)
        {

            rb.MovePosition(rb.position + direction * speed);

            if (Vector3.Distance(transform.position, startPosition) > range)
            {
                StartCoroutine(DestroyProjectile(true));
            }
        }
    }

    IEnumerator DestroyProjectile(bool mustSpawnCarrot)
    {
        destroyed = true;

        sr.enabled = false;
        triggerCC.enabled = false;
        collisionCC.enabled = false;

        if (mustSpawnCarrot)
        {
            GameObject.Instantiate(carrot).transform.position = transform.position;
        }

        trail.emitting = false;
        yield return new WaitForSeconds(trail.time);

        Destroy(gameObject);
    }
}
