using UnityEngine;
using UnityEngine.UI;
using Core.UI;

namespace Core.Players
{
    public class PlayerOutOfBound : MonoBehaviour
    {
        [SerializeField]
        private GameObject OOBPrefab;

        [SerializeField]
        private SpriteRenderer playerRenderer;
        [SerializeField]
        private Player playerScript;

        private OutOfBoundIcon oobIcon;


        public void Initialize(Transform oobParent)
        {
            oobIcon = GameObject.Instantiate(OOBPrefab, oobParent).GetComponent<OutOfBoundIcon>();
            oobIcon.SetSprite(playerScript.GetSprites().HeadSprite);
            oobIcon.gameObject.SetActive(false);
        }

        void Update()
        {
            if (playerRenderer.isVisible)
            {
                oobIcon.gameObject.SetActive(false);
            }
            else
            {
                oobIcon.gameObject.SetActive(true);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(playerRenderer.transform.position);
                Rect screenRect = Camera.main.pixelRect;
                Vector2 oobIconPos = new Vector2(
                    Mathf.Clamp((screenPos.x / screenRect.width * 1920) - 960, -910, 910), 
                    Mathf.Clamp((screenPos.y / screenRect.height * 1080) - 540, -490, 490)
                    );
                oobIcon.SetPosition(oobIconPos);

                oobIcon.CalculateArrowPosition(playerRenderer.transform.position);
            }
        }

        void OnDestroy()
        {
            if(oobIcon)
            {
                Destroy(oobIcon.gameObject);
            }
        }
    }
}
