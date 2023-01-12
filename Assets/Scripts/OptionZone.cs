using UnityEngine;
using UnityEngine.InputSystem;

public class OptionZone : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    LayerMask playerMask;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
        menuManager.PlayerEnterOptionZone(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
        menuManager.PlayerLeaveOptionZone(collision.gameObject);
    }
}
