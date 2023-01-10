using UnityEngine;
using UnityEngine.InputSystem;

public class ControllersManager : MonoBehaviour
{
    public void OnJoin(PlayerInput controller)
    {
        print(controller.name + " joined.");
    }

    public void OnLeft(PlayerInput controller)
    {
        print(controller.name + " left.");
    }
}
