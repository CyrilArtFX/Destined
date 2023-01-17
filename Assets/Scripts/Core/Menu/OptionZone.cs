using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Core.Menu
{
    public class OptionZone : MonoBehaviour
    {
        [SerializeField]
        LayerMask playerMask;

        List<GameObject> playersInOptionZone = new();

        [SerializeField]
        private GameObject optionsHUD;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
            PlayerEnterOptionZone(collision.gameObject);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
            PlayerLeaveOptionZone(collision.gameObject);
        }

        void Start()
        {
            optionsHUD.SetActive(false);
        }
        public void PlayerEnterOptionZone(GameObject player)
        {
            playersInOptionZone.Add(player);

            optionsHUD.SetActive(true);
        }

        public void PlayerLeaveOptionZone(GameObject player)
        {
            if (playersInOptionZone.Contains(player))
            {
                playersInOptionZone.Remove(player);
            }

            if (playersInOptionZone.Count == 0)
            {
                optionsHUD.SetActive(false);
            }
        }
    }
}
