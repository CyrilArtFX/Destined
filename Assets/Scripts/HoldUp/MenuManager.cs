using Core.Players;
using System.Collections.Generic;
using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Menu Manager")]
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Transform startPos;

        void Start()
        {
            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach(Player player in players)
            {
                player.Controller.SetInCinematic(false);
                player.transform.position = startPos.position;
            }
        }
    }
}
