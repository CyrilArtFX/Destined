using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

namespace Core.Players
{
    public class Player : MonoBehaviour
    {
        public PlayerController Controller => controller;

        [SerializeField]
        private TextMeshPro nameText;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private string controllerName;
        private PlayerSprite sprites;
        private int playerIndex;

        public int Index => playerIndex;

        [SerializeField]
        private PlayerController controller;

        private bool menuMode = true;
        public bool IsMenuMode => menuMode;

        public void InitializePlayer(string newControllerName, int newPlayerIndex)
        {
            controllerName = newControllerName;
            playerIndex = newPlayerIndex;
            nameText.text = "Player " + playerIndex;
        }

        public void ChangePlayerIndex(int newPlayerIndex)
        {
            playerIndex = newPlayerIndex;
            nameText.text = "Player " + playerIndex;
        }

        public void SetSprite(PlayerSprite newSprites)
        {
            sprites = newSprites;
            spriteRenderer.sprite = sprites.BodySprite;
        }

        public void SetToPlayMode()
        {
            menuMode = false;
        }

        public void SetToMenuMode()
        {
            menuMode = true;
        }

        public void Deconnect(InputAction.CallbackContext ctx)
        {
            if (!menuMode) return;

            Remove();
        }

        public void Deconnect()
        {
            if (!menuMode)
            {
                nameText.text = "Disconnected";
                PlayersManager.instance.PlayerDeconnexion(controllerName);

                return;
            }

            Remove();
        }

        public void Remove()
        {
            //  manage quit
            PlayersManager.instance.PlayerLeft(this);

            //  destroy
            Destroy(gameObject);
        }

        public void Reconnect()
        {
            if (!menuMode)
            {
                nameText.text = "Player " + playerIndex;
                PlayersManager.instance.PlayerReconnexion(controllerName);
            }
        }

        public string GetControllerName()
        {
            return controllerName;
        }

        public PlayerSprite GetSprites()
        {
            return sprites;
        }
    }
}
