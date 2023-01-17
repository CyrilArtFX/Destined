using Core.Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Core
{
    public abstract class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public bool IsFirstTimeLobby { get; protected set; }

        [SerializeField, Scene]
        private string menuScene;

        private void Awake()
        {
            instance = this;
            IsFirstTimeLobby = true;
        }

        void Start()
        {
            //  hook to scene loading
            SceneManager.sceneLoaded += OnSceneLoaded;

            int scenesOpened = SceneManager.sceneCount;
            if (scenesOpened > 1)
            {
                for (int i = 1; i < scenesOpened; i++)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
            }
            SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        public virtual void StartGame()
        {
        }

        public List<Player> GetPlayers()
        {
            return PlayersManager.instance.GetPlayers();
        }
    }
}
