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

    void Start()
    {
        ResetProgressBar();
        options.SetActive(false);
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

        if(playersInOptionZone.Count == 0)
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
