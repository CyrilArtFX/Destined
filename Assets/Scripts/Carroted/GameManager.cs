using Core.Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Carroted
{
    [AddComponentMenu("Scripts/Carroted Game Manager")]
    public class GameManager : Core.GameManager
    {
        private List<PlayerScore> playersScores = new();

        public override void StartGame()
        {
            IsFirstTimeLobby = false;

            PlayersManager.instance.SwitchToPlayMode();
            SceneManager.UnloadSceneAsync("Menu");
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

            List<Player> playerList = PlayersManager.instance.GetPlayers();
            foreach (Player player in playerList)
            {
                player.Inventory.ClearInventory();
                player.Controller.ClearEffects();
            }
        }

        public void ReturnToLobby(List<PlayerScore> scores)
        {
            playersScores = scores;

            SceneManager.UnloadSceneAsync("GameScene");
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);

            List<Player> playerList = PlayersManager.instance.GetPlayers();
            foreach (Player player in playerList)
            {
                player.Inventory.ClearInventory();
                player.Controller.ClearEffects();
            }
        }

        public List<PlayerScore> GetPlayersScores()
        {
            return playersScores;
        }
    }
}
