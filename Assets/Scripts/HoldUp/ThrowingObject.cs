using UnityEngine;

namespace HoldUp
{
    public class ThrowingObject : MonoBehaviour
    {
        private AnimationCurve powerCurve;
        private float timeForMinimalPower;

        private CircleCollider2D cc;
        private Rigidbody2D rb;

        private Vector2 initialVelocity;
        private float initialVelocitySpeed;
        private float timer;

        void Awake()
        {
            cc = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(float throwingPower, Vector2 throwingDirection, AnimationCurve throwingPowerCurve, float throwingTimeForMinimalPower, Collider2D ownerCollider)
        {
            powerCurve = throwingPowerCurve;
            timeForMinimalPower = throwingTimeForMinimalPower;

            Physics2D.IgnoreCollision(cc, ownerCollider);

            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(throwingDirection.y, throwingDirection.x) * Mathf.Rad2Deg));

            rb.AddForce(throwingDirection * throwingPower);
        }

        public void Explode()
        {
            print("prout");
            Destroy(gameObject);
        }

        void Update()
        {
            if(initialVelocity == Vector2.zero && rb.velocity != Vector2.zero)
            {
                initialVelocity = rb.velocity.normalized;
                initialVelocitySpeed = rb.velocity.magnitude;
            }

            timer = Mathf.Min(timer + Time.deltaTime, timeForMinimalPower);

            rb.velocity = rb.velocity.normalized * initialVelocitySpeed * powerCurve.Evaluate(timer / timeForMinimalPower);
        }
    }
}
