using Core.Players;
using System.Collections;
using System.Collections.Generic;
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
            List<Player> players = PlayersManager.instance.GetPlayers();

            if (van != null)
            {
                //  set sprite renderers
                for (int i = 0; i < van.CharactersRenderers.Length; i++)
                {
                    if (i < players.Count)
                    {
                        van.CharactersRenderers[i].sprite = players[i].GetSprites().BodySprite;
                        van.CharactersRenderers[i].enabled = true;
                    }
                    else
                    {
                        van.CharactersRenderers[i].enabled = false;
                    }
                }

                //  wait animation end
                yield return van.CoroutineStartAnimation();
            }

            for (int i = 0; i < van.CharactersRenderers.Length; i++)
            {
                if (i < players.Count)
                {
                    Player player = players[i];
                    player.transform.position = van.CharactersRenderers[i].transform.position;
                    playerControllers.Add(player.Controller as PlayerController);
                }

                van.CharactersRenderers[i].enabled = false;
            }

            playersCount = players.Count;
            playersDead = 0;
            globalMessageText.text = "";
            globalMessageBackground.SetActive(false);
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
