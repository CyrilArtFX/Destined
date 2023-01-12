using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private List<Storage> storages = new();

    private List<Player> playerList = new();

    void Start()
    {
        playerList = GameManager.instance.GetPlayers();
        int players = playerList.Count;

        for(int i = 0; i < storages.Count; i++)
        {
            if(i < players)
            {
                storages[i].AssignPlayer(playerList[i]);
                Physics2D.IgnoreCollision(storages[i].getCollider(), playerList[i].gameObject.GetComponent<Collider2D>(), true);
                playerList[i].transform.position = storages[i].getSpawnPosition();
            }
            else
            {
                storages[i].gameObject.SetActive(false);
            }
        }
    }
}
