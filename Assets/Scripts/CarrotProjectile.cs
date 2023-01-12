using System.Collections;
using UnityEngine;

public class CarrotProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float range;


    private Vector2 direction;

    private Vector3 startPosition;

    private SpriteRenderer sr;
    private TrailRenderer trail;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;

        startPosition = transform.position;

        transform.right = newDirection;
    }

    private void Update()
    {
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
        sr.enabled = false;

        trail.emitting = false;
        yield return new WaitForSeconds(trail.time);

        Destroy(gameObject);
    }
}
