using Core.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace HoldUp
{
    public class BankManager : MonoBehaviour
    {
        [SerializeField]
        private Transform playerSpawnPosition;
        [SerializeField]
        private Transform wantLobbyPosition;

        [SerializeField]
        private DepositArea depositArea;
        [SerializeField]
        private TextMeshProUGUI globalMessageText;
        [SerializeField]
        private GameObject globalMessageBackground;
        
        [SerializeField]
        private Van van;

        private int playersCount;
        private int playersDead;
        private List<PlayerController> playersWantLobby = new();

        private List<PlayerController> playerControllers = new();


        public static BankManager instance;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            StartCoroutine(CoroutineStartPlayerAnimation());
        }

        IEnumerator CoroutineStartPlayerAnimation()
        {
            globalMessageText.text = "";
            globalMessageBackground.SetActive(false);

            //  setup players
            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach (Player player in players)
            {
                player.transform.position = playerSpawnPosition.position;
                playerControllers.Add(player.Controller as PlayerController);
            }

            if (van != null)
            {
                //  setup renderers & players
                List<Transform> animated_characters = new();
                for (int i = 0; i < van.CharactersRenderers.Length; i++)
                {
                    SpriteRenderer renderer = van.CharactersRenderers[i];
                    if (i < players.Count)
                    {
                        Player player = players[i];
                        
                        PlayerController controller = player.Controller as PlayerController;
                        controller.SetInCinematic(true);
                        controller.HideVisually();

                        renderer.sprite = player.Renderer.sprite;
                        renderer.enabled = true;
                        animated_characters.Add(renderer.transform);
                    }
                    else
                    {
                        renderer.enabled = false;
                    }
                }

                //  center camera on animated characters
                CameraFollow camera = (GameManager.instance as GameManager).Camera;
                camera.SetTargets(animated_characters, true);

                //  wait animation end
                yield return van.CoroutineStartAnimation();

                //  setup players ready
                for (int i = 0; i < van.CharactersRenderers.Length; i++)
                {
                    SpriteRenderer renderer = van.CharactersRenderers[i];
                    if (i < players.Count)
                    {
                        Player player = players[i];
                        
                        PlayerController controller = player.Controller as PlayerController;
                        controller.SetInCinematic(false);
                        controller.ShowVisually();

                        player.transform.position = renderer.transform.position + new Vector3(0.0f, 0.5f);
                    }

                    renderer.enabled = false;
                }

                //  give camera back to players
                camera.SetTargets(players);
            }
            else
            {
                Debug.LogWarning("BankManager: Van == null, no start animation");
            }

            playersCount = players.Count;
            playersDead = 0;
        }

        public void PlayerDead()
        {
            playersDead++;
            if (playersDead == playersCount)
            {
                ReturnToLobby(true);
            }
        }

        public void PlayerRevived()
        {
            playersDead--;
        }

        public void PlayerWantToLobby(PlayerController controller)
        {
            controller.Inventory.EquipAndDrop(true);
            controller.transform.position = wantLobbyPosition.position;
            controller.SetInCinematic(true);

            playersWantLobby.Add(controller);
            if (playersWantLobby.Count == playersCount)
            {
                ReturnToLobby(false);
            }
        }

        public void PlayerStopWantLobby(PlayerController controller)
        {
            controller.transform.position = playerSpawnPosition.position;
            controller.SetInCinematic(false);

            if (playersWantLobby.Contains(controller))
            {
                playersWantLobby.Remove(controller);
            }
        }

        private void ReturnToLobby(bool playersDead)
        {
            if (playersDead)
            {
                StartCoroutine(DeathReturnToLobby());
            }
            else
            {
                StartCoroutine(PlayersReturnToLobby());
            }
        }

        private IEnumerator PlayersReturnToLobby()
        {
            globalMessageBackground.SetActive(true);
            globalMessageText.text = "Players went back home with what they took.\nScore : " + depositArea.Score;
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.SetInCinematic(true);
            }
            yield return new WaitForSeconds(2.0f);
            (GameManager.instance as GameManager).ReturnToLobby(depositArea.Score);
        }

        //  currently the same but we will do different cinematic depending if the players are dead or not

        private IEnumerator DeathReturnToLobby()
        {
            globalMessageBackground.SetActive(true);
            globalMessageText.text = "Players all died...\nScore : 0";
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.SetInCinematic(true);
            }
            yield return new WaitForSeconds(2.0f);
            (GameManager.instance as GameManager).ReturnToLobby(0);
        }
    }
}
