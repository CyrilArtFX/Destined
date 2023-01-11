using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    List<PlayerInput> playersInStartZone = new();

    [SerializeField]
    private float timeForStart;

    private float timer;

    [SerializeField]
    private StartZone startZone;

    void Start()
    {
        ResetProgressBar();
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

    void Update()
    {
        if (playersInStartZone.Count > 1 && playersInStartZone.Count == PlayersManager.instance.GetNumberOfPlayers())
        {
            timer += Time.deltaTime;
            startZone.ChangeProgressBar(timer / timeForStart);
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
}
