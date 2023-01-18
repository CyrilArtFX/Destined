using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class OutOfBoundIcon : MonoBehaviour
    {
        [SerializeField]
        private Image playerSprite;
        [SerializeField]
        private RectTransform arrow;

        private RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetSprite(Sprite sprite)
        {
            playerSprite.sprite = sprite;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }
        
        public void CalculateArrowPosition(Vector2 playerPosition)
        {
            Vector2 arrowDirection = playerPosition - (Vector2)rectTransform.position;
            arrowDirection.Normalize();

            arrow.anchoredPosition = arrowDirection * 37.5f;
            arrow.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(-arrowDirection.x, arrowDirection.y) * Mathf.Rad2Deg));
        }
    }
}
