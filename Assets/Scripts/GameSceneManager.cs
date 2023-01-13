using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private List<Storage> storages = new();

    [SerializeField]
    private float gameTimer;
    [SerializeField]
    private TextMeshProUGUI gameTimerText;

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

    void Update()
    {
        gameTimer -= Time.deltaTime;
        gameTimerText.text = ((int)gameTimer).ToString();
    }
}
