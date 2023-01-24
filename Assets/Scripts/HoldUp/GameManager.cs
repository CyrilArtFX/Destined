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
        [SerializeField, Scene]
        protected string gameScene;

        [SerializeField]
        private CameraFollow gameCamera;


        public override void StartGame()
        {
            PlayersManager.instance.SwitchToPlayMode();
            SceneManager.UnloadSceneAsync(menuScene);
            SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);

            List<Player> players = PlayersManager.instance.GetPlayers();
            foreach(Player player in players)
            {
                player.Controller.ClearEffects();
            }

            gameCamera.EnableFollowMode(players);
        }
    }
}
