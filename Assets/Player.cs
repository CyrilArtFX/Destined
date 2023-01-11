using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro nameText;

    private string controllerName;

    public void InitializePlayer(string newControllerName, int playerIndex)
    {
        controllerName = newControllerName;
        nameText.text = "Player " + playerIndex;
    }

    public void ChangePlayerIndex(int playerIndex)
    {
        nameText.text = "Player " + playerIndex;
    }

    public void Deconnect(InputAction.CallbackContext ctx)
    {
        PlayersManager.instance.PlayerLeft(this);
        Destroy(gameObject);
    }

    public string GetControllerName()
    {
        return controllerName;
    }
}
