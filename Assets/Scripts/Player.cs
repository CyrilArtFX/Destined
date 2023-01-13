using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Player : MonoBehaviour
{
    public Inventory Inventory => inventory;
    public PlayerController Controller => controller;

    [SerializeField]
    private TextMeshPro nameText;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private string controllerName;
    private Sprite sprite;
    private int playerIndex;

    public int Index => playerIndex;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerController controller;
    
    private bool menuMode = true;

    public void InitializePlayer(string newControllerName, int newPlayerIndex)
    {
        controllerName = newControllerName;
        playerIndex = newPlayerIndex;
        nameText.text = "Player " + playerIndex;
    }

    public void ChangePlayerIndex(int newPlayerIndex)
    {
        playerIndex = newPlayerIndex;
        nameText.text = "Player " + playerIndex;
    }

    public void SetSprite(Sprite newSprite)
    {
        sprite = newSprite;
        spriteRenderer.sprite = sprite;
    }

    public void SetToPlayMode()
    {
        menuMode = false;
    }

    public void SetToMenuMode()
    {
        menuMode = true;
    }

    public void Deconnect(InputAction.CallbackContext ctx)
    {
        if (!menuMode) return;

        Remove();
    }

    public void Deconnect()
    {
        if(!menuMode)
        {
            nameText.text = "Disconnected";
            PlayersManager.instance.PlayerDeconnexion(controllerName);

            return;
        }

        Remove();
    }

    public void Remove()
    {
        //  drop items
        for ( int i = 0; i <= inventory.ItemsCount; i++ )
            inventory.DropLastItem();

        //  manage quit
        PlayersManager.instance.PlayerLeft(this);
        
        //  destroy
        Destroy(gameObject);
    }

    public void Reconnect()
    {
        if(!menuMode)
        {
            nameText.text = "Player " + playerIndex;
            PlayersManager.instance.PlayerReconnexion(controllerName);
        }
    }

    public string GetControllerName()
    {
        return controllerName;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}
