using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Inventory Inventory => inventory;

    [SerializeField]
    private TextMeshPro nameText;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private string controllerName;
    private Sprite sprite;

    [SerializeField]
    private Inventory inventory;

    public void InitializePlayer(string newControllerName, int playerIndex)
    {
        controllerName = newControllerName;
        nameText.text = "Player " + playerIndex;
    }

    public void ChangePlayerIndex(int playerIndex)
    {
        nameText.text = "Player " + playerIndex;
    }

    public void SetSprite(Sprite newSprite)
    {
        sprite = newSprite;
        spriteRenderer.sprite = sprite;
    }

    public void Deconnect(InputAction.CallbackContext ctx)
    {
        PlayersManager.instance.PlayerLeft(this);
        Destroy(gameObject);
    }

    public void Deconnect()
    {
        PlayersManager.instance.PlayerLeft(this);
        Destroy(gameObject);
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
