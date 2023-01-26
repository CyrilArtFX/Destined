using TMPro;
using UnityEngine;

namespace HoldUp
{
    public class DepositArea : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro debugText;

        public int Score => itemCounter;
        private int itemCounter;

        void Start()
        {
            itemCounter = 0;
            debugText.text = itemCounter.ToString();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<AutoDropItem>(out AutoDropItem item))
            {
                if (item.AutoDrop())
                {
                    itemCounter++;
                    debugText.text = itemCounter.ToString();
                }
            }
        }
    }
}
