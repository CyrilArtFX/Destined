using UnityEngine;

namespace HoldUp
{
    public class ThrowingItem : Item
    {
        [SerializeField]
        private ThrowingObject associatedObject;

        [SerializeField]
        private float throwingPower;
        [SerializeField]
        private AnimationCurve throwingPowerCurve;
        [SerializeField]
        private float throwingTimeForMinimalPower;

        [SerializeField]
        private float explosionRadius, explosionDamages;

        [SerializeField]
        private Sprite remoteSprite;



        private new SpriteRenderer renderer;
        private bool remote = false;
        private ThrowingObject objectToThrow;

        void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }


        public override void OnUsePressed()
        {
            if (!isHandMode) return;

            if (remote)
            {
                objectToThrow.Explode();

                if(inventory)
                {
                    inventory.DestroyItemInHand();
                }
            }
            else
            {
                objectToThrow = GameObject.Instantiate(associatedObject.gameObject, GameManager.instance.transform).GetComponent<ThrowingObject>();
                objectToThrow.transform.position = transform.position;
                if (owner.TryGetComponent(out Collider2D ownerCollider))
                {
                    objectToThrow.Initialize(throwingPower, direction, throwingPowerCurve, throwingTimeForMinimalPower, ownerCollider, explosionRadius, explosionDamages);
                }

                renderer.sprite = remoteSprite;
                remote = true;
            }
        }

        public override void Drop()
        {
            if (!isHandMode) return;

            if (remote)
            {
                objectToThrow.Explode();
            }
            else
            {
                base.Drop();
            }
        }

        public void ForceInstantExplosion()
        {
            if (remote) return;

            objectToThrow = GameObject.Instantiate(associatedObject.gameObject, GameManager.instance.transform).GetComponent<ThrowingObject>();
            objectToThrow.transform.position = transform.position;
            objectToThrow.Initialize(0, Vector2.zero, AnimationCurve.Constant(0.0f, 1.0f, 0.0f), 1.0f, null, explosionRadius, explosionDamages);
            remote = true;
            objectToThrow.Explode();

            if (inventory)
            {
                inventory.DestroyItemInHand();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
