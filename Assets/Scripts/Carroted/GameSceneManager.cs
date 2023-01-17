using Core.Players;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Carroted
{
    public struct PlayerScore : IComparer<PlayerScore>
    {
        public Player player;
        public int score;

        public int Compare(PlayerScore x, PlayerScore y)
        {
            return x.score.CompareTo(y.score);
        }
    }

    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField]
        private List<Storage> storages = new();

        [SerializeField]
        private float gameTimer;
        [SerializeField]
        private TextMeshProUGUI gameTimerText;

        public float GameTimer => gameTimer;

        private List<Player> playerList = new();

        [SerializeField]
        private Burrow burrow;
        [SerializeField]
        private BurrowData twoPlayersBurrowData;
        [SerializeField]
        private BurrowData threePlayersBurrowData;
        [SerializeField]
        private BurrowData fourPlayersBurrowData;

        public static GameSceneManager instance;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            playerList = (GameManager.instance as GameManager).GetPlayers();
            int players = playerList.Count;

            for (int i = 0; i < storages.Count; i++)
            {
                if (i < players)
                {
                    storages[i].AssignPlayer(playerList[i]);
                    Physics2D.IgnoreCollision(storages[i].GetCollider(), playerList[i].gameObject.GetComponent<Collider2D>(), true);
                    playerList[i].transform.position = storages[i].GetSpawnPosition();
                }
                else
                {
                    storages[i].gameObject.SetActive(false);
                }
            }

            switch (players)
            {
                case 2:
                    burrow.Data = twoPlayersBurrowData;
                    break;
                case 3:
                    burrow.Data = threePlayersBurrowData;
                    break;
                case 4:
                    burrow.Data = fourPlayersBurrowData;
                    break;
            }
            burrow.Initialize();
        }

        void Update()
        {
            gameTimer -= Time.deltaTime;
            gameTimerText.text = ((int)gameTimer).ToString();

            if (gameTimer < 0.0f)
            {
                List<PlayerScore> scores = new();
                for (int i = 0; i < playerList.Count; i++)
                {
                    scores.Add(new PlayerScore { player = playerList[i], score = storages[i].GetPlayerScore() });
                }

                (GameManager.instance as GameManager).ReturnToLobby(scores);
            }
        }
    }
}
