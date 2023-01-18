using Core.Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Carroted
{
    [AddComponentMenu("Scripts/Carroted Game Manager")]
    public class GameManager : Core.GameManager
    {
        private List<PlayerScore> playersScores = new();

        [SerializeField, Scene]
        protected string gameScene;

        public override void StartGame()
        {
            IsFirstTimeLobby = false;

            PlayersManager.instance.SwitchToPlayMode();
            SceneManager.UnloadSceneAsync(menuScene);
            SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);

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

            SceneManager.UnloadSceneAsync(gameScene);
            SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);

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
