using UnityEngine;
using UnityEngine.InputSystem;

public class MenuZone : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        menuManager.PlayerEnterMenuZone(collision.gameObject); 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        menuManager.PlayerLeaveMenuZone(collision.gameObject);
    }
}
