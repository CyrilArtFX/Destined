using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Player : MonoBehaviour
{
    public Inventory Inventory => inventory;

    [SerializeField]
    private TextMeshPro nameText;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private string controllerName;
    private Sprite sprite;
    private int playerIndex;

    [SerializeField]
    private Inventory inventory;
    
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

    public void Deconnect(InputAction.CallbackContext ctx)
    {
        if (!menuMode) return;

        PlayersManager.instance.PlayerLeft(this);
        Destroy(gameObject);
    }

    public void Deconnect()
    {
        if(!menuMode)
        {
            nameText.text = "Disconnected";
            PlayersManager.instance.PlayerDeconnexion(controllerName);

            return;
        }

        PlayersManager.instance.PlayerLeft(this);
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
