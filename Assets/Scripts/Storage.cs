using UnityEngine;
using TMPro;

public class Storage : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private TextMeshPro debugText;

    [SerializeField]
    private Transform playerSpawnPoint;


    private Player assignedPlayer;
    private int carrotStored = 0;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != assignedPlayer.gameObject) return;

        assignedPlayer.Controller.SetInsideSafeZone(true);

        foreach ( Collectible item in assignedPlayer.Inventory.Items )
        { 
            carrotStored += item.Score;
        }
        assignedPlayer.Inventory.ClearInventory();

        debugText.text = carrotStored.ToString();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != assignedPlayer.gameObject) return;

        assignedPlayer.Controller.SetInsideSafeZone(false);
    }

    public void AssignPlayer(Player playerToAssign)
    {
        assignedPlayer = playerToAssign;
    }

    public BoxCollider2D getCollider()
    {
        return boxCollider;
    }

    public Vector3 getSpawnPosition()
    {
        return playerSpawnPoint.position;
    }
}
