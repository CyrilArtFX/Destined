using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    List<PlayerInput> playersInStartZone = new();

    List<GameObject> playersInOptionZone = new();

    [SerializeField]
    private float timeForStart;

    private float timer;

    [SerializeField]
    private StartZone startZone;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private List<Podium> podiums;

    private Dictionary<int, List<Player>> playersOnPodium = new();

    void Start()
    {
        ResetProgressBar();
        options.SetActive(false);

        if (GameManager.instance.IsFirstTimeLobby) return;

        //  draw podium, put the players on it and play the 'cinematic'

        List<PlayerScore> playersScores = GameManager.instance.GetPlayersScores();

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
                    playersOnPodium[place].Add(playersScores[i].player);
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
            foreach (Player player in playersOnPodium[i])
            {
                podiums[podiumUsed].gameObject.SetActive(true);
                Vector3 playerPosOnPodium = podiums[podiumUsed].SetPodium(i);
                player.transform.position = playerPosOnPodium;
                podiumUsed++;
            }
        }

        StartCoroutine(PodiumCinematic());
    }

    public void PlayerEnterStartZone(PlayerInput player)
    {
        playersInStartZone.Add(player);
    }

    public void PlayerLeaveStartZone(PlayerInput player)
    {
        if (playersInStartZone.Contains(player))
        {
            playersInStartZone.Remove(player);
        }

        ResetProgressBar();
    }

    public void PlayerEnterOptionZone(GameObject player)
    {
        playersInOptionZone.Add(player);

        options.SetActive(true);
    }

    public void PlayerLeaveOptionZone(GameObject player)
    {
        if (playersInOptionZone.Contains(player))
        {
            playersInOptionZone.Remove(player);
        }

        if (playersInOptionZone.Count == 0)
        {
            options.SetActive(false);
        }
    }

    void Update()
    {
        if (playersInStartZone.Count > 1 && playersInStartZone.Count == PlayersManager.instance.GetNumberOfPlayers())
        {
            timer += Time.deltaTime;
            startZone.ChangeProgressBar(timer / timeForStart);
        }

        if (timer > 0.0f && playersInStartZone.Count < PlayersManager.instance.GetNumberOfPlayers())
        {
            ResetProgressBar();
        }

        if (timer > timeForStart)
        {
            GameManager.instance.StartGame();
        }
    }

    private void ResetProgressBar()
    {
        timer = 0.0f;
        startZone.ChangeProgressBar(0.0f);
    }

    private IEnumerator PodiumCinematic()
    {
        List<Player> players = PlayersManager.instance.GetPlayers();

        foreach(Player player in players)
        {
            player.Controller.SetInCinematic(true);
        }

        yield return new WaitForSeconds(5.0f);

        foreach(Podium podium in podiums)
        {
            podium.gameObject.SetActive(false);
        }

        foreach (Player player in players)
        {
            player.Controller.SetInCinematic(false);
        }
        PlayersManager.instance.SwitchToMenuMode();
    }
}
