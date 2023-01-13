using UnityEngine;
using TMPro;

public class Storage : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private Transform playerSpawnPoint;

    [SerializeField]
    private BoxCollider2D visualStorageZone;

    [SerializeField]
    private GameObject visualStoredCarrot;

    [SerializeField]
    private GameObject visualStoredGoldenCarrot;


    private Player assignedPlayer;
    private int carrotStored = 0;



    private float vsMinX;
    private float vsMaxX;
    private float vsMinY;
    private float vsMaxY;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != assignedPlayer.gameObject) return;

        assignedPlayer.Controller.SetInsideSafeZone(true);

        foreach (Collectible item in assignedPlayer.Inventory.Items)
        {
            carrotStored += item.Score;
            SpawnVisualStoredCarrot(item.Type == Collectible.ItemType.GOLD_CARROT);
        }
        assignedPlayer.Inventory.ClearInventory();
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

    public BoxCollider2D GetCollider()
    {
        return boxCollider;
    }

    public Vector3 GetSpawnPosition()
    {
        return playerSpawnPoint.position;
    }

    public int GetPlayerScore()
    {
        return carrotStored;
    }

    void Start()
    {
        vsMinX = visualStorageZone.bounds.center.x - visualStorageZone.bounds.extents.x;
        vsMaxX = visualStorageZone.bounds.center.x + visualStorageZone.bounds.extents.x;
        vsMinY = visualStorageZone.bounds.center.y - visualStorageZone.bounds.extents.y;
        vsMaxY = visualStorageZone.bounds.center.y + visualStorageZone.bounds.extents.y;
    }

    private void SpawnVisualStoredCarrot(bool spawnGolden)
    {
        float rdmX = Random.Range(vsMinX, vsMaxX);
        float rdmY = Random.Range(vsMinY, vsMaxY);

        print(rdmX + "   " + vsMinX + "   " + vsMaxX);

        GameObject carrot = GameObject.Instantiate(spawnGolden ? visualStoredGoldenCarrot : visualStoredCarrot, visualStorageZone.transform, true);
        carrot.transform.position = new Vector2(rdmX, rdmY);
    }
}
