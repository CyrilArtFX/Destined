using UnityEngine;

namespace HoldUp
{
    public class DepositArea : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider2D visualDepositRect;
        [SerializeField]
        private Transform visualDepositTransform;

        public int Score => itemCounter;
        private int itemCounter;


        private float vsMinX,  vsMaxX, vsMinY, vsMaxY;

        void Start()
        {
            itemCounter = 0;

            vsMinX = visualDepositRect.bounds.center.x - visualDepositRect.bounds.extents.x;
            vsMaxX = visualDepositRect.bounds.center.x + visualDepositRect.bounds.extents.x;
            vsMinY = visualDepositRect.bounds.center.y - visualDepositRect.bounds.extents.y;
            vsMaxY = visualDepositRect.bounds.center.y + visualDepositRect.bounds.extents.y;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<AutoDropItem>(out AutoDropItem item))
            {
                GameObject visualBag = item.AutoDrop();
                if (visualBag)
                {
                    itemCounter++;

                    float rdmX = Random.Range(vsMinX, vsMaxX);
                    float rdmY = Random.Range(vsMinY, vsMaxY);

                    GameObject bag = GameObject.Instantiate(visualBag, visualDepositTransform);
                    bag.transform.position = new Vector2(rdmX, rdmY);
                }
            }
        }
    }
}
