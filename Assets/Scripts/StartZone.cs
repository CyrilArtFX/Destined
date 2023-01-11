using UnityEngine;
using UnityEngine.InputSystem;

public class StartZone : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        menuManager.PlayerEnterStartZone(collision.gameObject.GetComponent<PlayerInput>()); 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        menuManager.PlayerLeaveStartZone(collision.gameObject.GetComponent<PlayerInput>());
    }
}
