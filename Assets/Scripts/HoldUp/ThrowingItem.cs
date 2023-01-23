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
            if(remote)
            {
                objectToThrow.Explode();

                playerController.Inventory.DestroyItemInHand();
            }
            else
            {
                objectToThrow = GameObject.Instantiate(associatedObject.gameObject, GameManager.instance.transform).GetComponent<ThrowingObject>();
                objectToThrow.transform.position = transform.position;
                objectToThrow.Initialize(throwingPower, Direction, throwingPowerCurve, throwingTimeForMinimalPower, playerController.GetComponent<Collider2D>(), explosionRadius, explosionDamages);

                renderer.sprite = remoteSprite;
                remote = true;
            }

        }

        public override void Drop()
        {
            if(remote)
            {
                objectToThrow.Explode();
            }
            else
            {
                base.Drop();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
