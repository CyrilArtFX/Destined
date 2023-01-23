using Core.Players;
using System.Collections.Generic;
using UnityEngine;

namespace HoldUp
{
    public class BankManager : MonoBehaviour
    {
        [SerializeField]
        private Transform playerSpawnPosition;

        void Start()
        {
            List<Player> players = PlayersManager.instance.GetPlayers();

            foreach(Player player in players)
            {
                player.transform.position = playerSpawnPosition.position;
            }
        }
    }
}
