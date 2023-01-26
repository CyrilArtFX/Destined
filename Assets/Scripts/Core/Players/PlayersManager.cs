using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Core.Players
{
    [System.Serializable]
    public struct PlayerSprite
    {
        public Sprite BodySprite;
        public Sprite HeadSprite;
    }

    public class PlayersManager : MonoBehaviour
    {
        private Dictionary<string, PlayerInput> controllers = new();
        private List<Player> players = new();

        private List<string> disconnectedControllers = new();


        [SerializeField]
        private List<PlayerSprite> playerSprites = new();

        [SerializeField, Tooltip("Leave this at none if you don't need the OOB system in your game")]
        private Transform playerOOBParent;

        private Dictionary<PlayerSprite, bool> sprites = new();

        public static PlayersManager instance;

        private bool menuMode = true;


        [SerializeField]
        private TextMeshProUGUI disconnectedControllersText;

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            foreach (PlayerSprite playerSprite in playerSprites)
            {
                sprites.Add(playerSprite, false);
            }
        }

        public void PlayerJoin(PlayerInput playerInput)
        {
            if (!menuMode)
            {
                Destroy(playerInput.gameObject);
                return;
            }

            int sameTypeIndex = 1;
            string deviceName = playerInput.devices[0].displayName;
            foreach (string controller in controllers.Keys)
            {
                if (deviceName + " " + sameTypeIndex == controller)
                {
                    sameTypeIndex++;
                }
            }

            deviceName += " " + sameTypeIndex;

            controllers.Add(deviceName, playerInput);

            Player player = playerInput.gameObject.GetComponent<Player>();
            players.Add(player);
            player.InitializePlayer(deviceName, players.Count);

            List<PlayerSprite> unusedSprites = new();
            foreach (PlayerSprite sprite in sprites.Keys)
            {
                if (!sprites[sprite])
                {
                    unusedSprites.Add(sprite);
                }
            }

            int rdm = Random.Range(0, unusedSprites.Count);
            player.SetSprite(unusedSprites[rdm]);
            sprites[unusedSprites[rdm]] = true;

            player.transform.position = transform.position;

            if(playerOOBParent)
            {
                player.GetComponent<PlayerOutOfBound>().Initialize(playerOOBParent);
            }
        }

        public void PlayerLeft(Player player)
        {
            players.Remove(player);
            ReorganizePlayers();
            controllers.Remove(player.GetControllerName());
            sprites[player.GetSprites()] = false;
        }

        public void PlayerDeconnexion(string controllerName)
        {
            if (menuMode)
            {
                return;
            }

            disconnectedControllers.Add(controllerName);

            UpdateDisconnectedControllersText();
        }

        public void PlayerReconnexion(string controllerName)
        {
            if (menuMode)
            {
                return;
            }

            disconnectedControllers.Remove(controllerName);

            UpdateDisconnectedControllersText();
        }

        private void UpdateDisconnectedControllersText()
        {
            int numberOfDisconnectedControllers = disconnectedControllers.Count;

            string str = "";

            if (numberOfDisconnectedControllers > 0)
            {
                str = "Controller";
                if (numberOfDisconnectedControllers > 1)
                {
                    str += "s";
                }

                for (int i = 0; i  < numberOfDisconnectedControllers; i++)
                {
                    str += " " + disconnectedControllers[i];

                    if (i < numberOfDisconnectedControllers - 1)
                    {
                        str += " and";
                    }
                }

                if (numberOfDisconnectedControllers > 1)
                {
                    str += " are disconnected.";
                }
                else
                {
                    str += " is disconnected.";
                }
            }

            disconnectedControllersText.text = str;
        }

        private void ReorganizePlayers()
        {
            List<Player> playersReorganized = new();

            foreach (Player player in players)
            {
                playersReorganized.Add(player);
                player.ChangePlayerIndex(playersReorganized.Count);
            }

            players.Clear();
            players = playersReorganized;
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public int GetNumberOfPlayers()
        {
            return players.Count;
        }

        public void SwitchToPlayMode()
        {
            menuMode = false;
            foreach (Player player in players)
            {
                player.SetToPlayMode();
            }
        }

        public void SwitchToMenuMode()
        {
            menuMode = true;
            foreach (Player player in players)
            {
                player.SetToMenuMode();
            }
        }
    }
}

