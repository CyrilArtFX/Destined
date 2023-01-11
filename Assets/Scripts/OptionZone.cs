using UnityEngine;
using UnityEngine.InputSystem;

public class OptionZone : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        menuManager.PlayerEnterOptionZone(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        menuManager.PlayerLeaveOptionZone(collision.gameObject);
    }
}
