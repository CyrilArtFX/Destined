using Core.Players;
using System.Collections;
using System.Collections.Generic;
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
            List<Player> players = PlayersManager.instance.GetPlayers();

            foreach (Player player in players)
            {
                player.transform.position = playerSpawnPosition.position;
                playerControllers.Add(player.Controller as PlayerController);
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
            print("players decided to go home");
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.SetInCinematic(true);
            }
            yield return new WaitForSeconds(1.0f);
            (GameManager.instance as GameManager).ReturnToLobby(depositArea.Score);
        }

        //  currently the same but we will do different cinematic depending if the players are dead or not

        private IEnumerator DeathReturnToLobby()
        {
            print("players are all dead...");
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.SetInCinematic(true);
            }
            yield return new WaitForSeconds(1.0f);
            (GameManager.instance as GameManager).ReturnToLobby(depositArea.Score);
        }
    }
}
