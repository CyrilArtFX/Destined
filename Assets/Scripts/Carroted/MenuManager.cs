using Core.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carroted
{
    [AddComponentMenu("Scripts/Carroted Menu Manager")]
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private List<Podium> podiums;

        private Dictionary<int, List<PlayerScore>> playersOnPodium = new();


        [SerializeField]
        private AudioSource menuMusic;
        [SerializeField]
        private AudioSource endGameMusic;

        void Start()
        {
            if (GameManager.instance.IsFirstTimeLobby) return;

            //  draw podium, put the players on it and play the 'cinematic'

            menuMusic.Stop();

            List<PlayerScore> playersScores = (GameManager.instance as GameManager).GetPlayersScores();

            playersScores.Sort(playersScores[0]);

            int place = 1;
            while (playersScores.Count > 0)
            {
                int topScore = playersScores[playersScores.Count - 1].score;
                for (int i = playersScores.Count - 1; i >= 0; i--)
                {
                    if (playersScores[i].score == topScore)
                    {
                        if (!playersOnPodium.ContainsKey(place))
                        {
                            playersOnPodium.Add(place, new());
                        }
                        playersOnPodium[place].Add(playersScores[i]);
                        playersScores.RemoveAt(i);
                    }
                }
                place++;
            }

            foreach (Podium podium in podiums)
            {
                podium.gameObject.SetActive(false);
            }

            int podiumUsed = 0;
            for (int i = 1; i < place; i++)
            {
                foreach (PlayerScore playerScore in playersOnPodium[i])
                {
                    podiums[podiumUsed].gameObject.SetActive(true);
                    Vector3 playerPosOnPodium = podiums[podiumUsed].SetPodium(i, playerScore.score);
                    playerScore.player.transform.position = playerPosOnPodium;
                    podiumUsed++;
                }
            }

            StartCoroutine(PodiumCinematic());
        }

        private IEnumerator PodiumCinematic()
        {
            List<Player> players = PlayersManager.instance.GetPlayers();

            foreach (Player player in players)
            {
                player.Controller.SetInCinematic(true);
            }

            if (endGameMusic.clip)
            {
                endGameMusic.Play();
                yield return new WaitForSeconds(endGameMusic.clip.length);
            }
            else
            {
                yield return new WaitForSeconds(5.0f);
            }

            foreach (Podium podium in podiums)
            {
                podium.gameObject.SetActive(false);
            }

            foreach (Player player in players)
            {
                player.Controller.SetInCinematic(false);
            }
            PlayersManager.instance.SwitchToMenuMode();

            menuMusic.Play();
            menuMusic.loop = true;
        }
    }
}
