using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public bool IsFirstTimeLobby => firstTimeLobby;
    private bool firstTimeLobby = true;


    private List<PlayerScore> playersScores = new();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int scenesOpened = SceneManager.sceneCount;
        if (scenesOpened > 1)
        {
            for (int i = 1; i < scenesOpened; i++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }

	void OnLevelWasLoaded( int level )
	{
		foreach ( Transform child in transform )
            Destroy( child.gameObject );
	}

	public void StartGame()
    {
        firstTimeLobby = false;

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

    public List<Player> GetPlayers()
    {
        return PlayersManager.instance.GetPlayers();
    }

    public List<PlayerScore> GetPlayersScores()
    {
        return playersScores;
    }
}
