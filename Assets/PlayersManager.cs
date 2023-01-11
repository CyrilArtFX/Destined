using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    Dictionary<string, PlayerInput> controllers = new();
    List<Player> players = new();

    public static PlayersManager instance;

    void Awake()
    {
        instance = this;
    }

    public void PlayerJoin(PlayerInput playerInput)
    {
        int sameTypeIndex = 1;
        string deviceName = playerInput.devices[0].displayName;
        foreach(string controller in controllers.Keys)
        {
            if(deviceName + " " + sameTypeIndex == controller)
            {
                sameTypeIndex++;
            }
        }

        deviceName += " " + sameTypeIndex;
        controllers.Add(deviceName, playerInput);


        /*print("New list :");
        foreach(string controller in controllers.Keys)
        {
            print(controller);
        }*/

        Player player = playerInput.gameObject.GetComponent<Player>();
        players.Add(player);
        player.InitializePlayer(deviceName, players.Count);
    }

    public void PlayerLeft(Player player)
    {
        players.Remove(player);
        ReorganizePlayers();
        controllers.Remove(player.GetControllerName());
    }

    private void ReorganizePlayers()
    {
        List<Player> playersReorganized = new();

        foreach(Player player in players)
        {
            playersReorganized.Add(player);
            player.ChangePlayerIndex(playersReorganized.Count);
        }

        players.Clear();
        players = playersReorganized;
    }
}
