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


    private Vector2 direction;
    private Vector3 startPosition;
    private GameObject owner;

    private SpriteRenderer sr;
    private TrailRenderer trail;
    private CircleCollider2D cc;

    private bool destroyed = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        cc = GetComponent<CircleCollider2D>();
    }

    public void Initialize(Vector2 newDirection, GameObject newOwner)
    {
        direction = newDirection;
        startPosition = transform.position;
        owner = newOwner;

        transform.right = newDirection;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;

        if (collision.gameObject == owner) return;

        collision.gameObject.GetComponent<PlayerController>().Stun(stunTime);

        StartCoroutine(DestroyProjectile());
    }

    private void Update()
    {
        if (destroyed) return;

        if(direction != Vector2.zero)
        {
            transform.position += (Vector3)direction * speed;

            if (Vector3.Distance(transform.position, startPosition) > range)
            {
                StartCoroutine(DestroyProjectile());
            }
        }
    }

    IEnumerator DestroyProjectile()
    {
        destroyed = true;

        sr.enabled = false;
        cc.enabled = false;

        trail.emitting = false;
        yield return new WaitForSeconds(trail.time);

        Destroy(gameObject);
    }
}
