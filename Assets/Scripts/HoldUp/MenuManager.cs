using Core.Players;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Menu Manager")]
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Transform startPos;

        [SerializeField]
        private TextMeshPro bestScoreText;

        void Start()
        {
            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach(Player player in players)
            {
                player.Controller.SetInCinematic(false);
                player.transform.position = startPos.position;
            }

            bestScoreText.text = "Best score :\n" + (GameManager.instance as GameManager).BestScore;
        }
    }
}
