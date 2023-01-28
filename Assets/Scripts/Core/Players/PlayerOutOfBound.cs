using UnityEngine;
using UnityEngine.UI;
using Core.UI;

namespace Core.Players
{
    public class PlayerOutOfBound : MonoBehaviour
    {
        public OutOfBoundIcon OOBIcon { get; private set; }

        [SerializeField]
        private GameObject OOBPrefab;

        [SerializeField]
        private SpriteRenderer playerRenderer;
        [SerializeField]
        private Player playerScript;

        public void Initialize(Transform oobParent)
        {
            OOBIcon = GameObject.Instantiate(OOBPrefab, oobParent).GetComponent<OutOfBoundIcon>();
            OOBIcon.SetSprite(playerScript.GetSprites().HeadSprite);
            OOBIcon.gameObject.SetActive(false);
        }

        void Update()
        {
            if (!OOBIcon) return;

            if (!playerRenderer.enabled || playerRenderer.isVisible)
            {
                OOBIcon.gameObject.SetActive(false);
            }
            else
            {
                OOBIcon.gameObject.SetActive(true);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(playerRenderer.transform.position);
                Rect screenRect = Camera.main.pixelRect;
                Vector2 oobIconPos = new Vector2(
                    Mathf.Clamp((screenPos.x / screenRect.width * 1920) - 960, -910, 910), 
                    Mathf.Clamp((screenPos.y / screenRect.height * 1080) - 540, -490, 490)
                    );
                OOBIcon.SetPosition(oobIconPos);

                OOBIcon.CalculateArrowPosition(playerRenderer.transform.position);
            }
        }

        void OnDestroy()
        {
            if(OOBIcon)
            {
                Destroy(OOBIcon.gameObject);
            }
        }
    }
}
