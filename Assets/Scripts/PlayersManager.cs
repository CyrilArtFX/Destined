using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    private Dictionary<string, PlayerInput> controllers = new();
    private List<Player> players = new();


    [SerializeField]
    private List<Sprite> playerSprites = new();
    
    private Dictionary<Sprite, bool> sprites = new();

    public static PlayersManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(Sprite playerSprite in playerSprites)
        {
            sprites.Add(playerSprite, false);
        }
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

        Player player = playerInput.gameObject.GetComponent<Player>();
        players.Add(player);
        player.InitializePlayer(deviceName, players.Count);

        List<Sprite> unusedSprites = new();
        foreach(Sprite sprite in sprites.Keys)
        {
            if (!sprites[sprite])
            {
                unusedSprites.Add(sprite);
            }
        }

        int rdm = Random.Range(0, unusedSprites.Count);
        player.SetSprite(unusedSprites[rdm]);
        sprites[unusedSprites[rdm]] = true;
    }

    public void PlayerLeft(Player player)
    {
        players.Remove(player);
        ReorganizePlayers();
        controllers.Remove(player.GetControllerName());
        sprites[player.GetSprite()] = false;
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

    public int GetNumberOfPlayers()
    {
        return players.Count;
    }
}
