using UnityEngine;
using Core.Players;
using Utility;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Game Manager")]
    public class GameManager : Core.GameManager
    {
        [SerializeField, Scene, Tooltip("Will select randomly one of these maps")]
        protected List<string> gameScenes;

        [SerializeField]
        private CameraFollow gameCamera;

        public int BestScore => PlayerPrefs.GetInt("BestScore");

        private string gameScene;


        public override void StartGame()
        {
            PlayersManager.instance.SwitchToPlayMode();
            SceneManager.UnloadSceneAsync(menuScene);
            int rdm = Random.Range(0, gameScenes.Count);
            gameScene = gameScenes[rdm];
            SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);

            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach (Player player in players)
            {
                player.Controller.ClearEffects();
            }

            gameCamera.EnableFollowMode(players);
        }

        public void ReturnToLobby(int score)
        {
            if (score > BestScore)
            {
                PlayerPrefs.SetInt("BestScore", score);
            }

            PlayersManager.instance.SwitchToMenuMode();
            SceneManager.UnloadSceneAsync(gameScene);
            SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);

            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach (Player player in players)
            {
                player.Controller.ClearEffects();
            }

            gameCamera.DisableFollowMode();
        }
    }
}
