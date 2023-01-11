using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    List<PlayerInput> playersInStartZone = new();

    public void PlayerEnterStartZone(PlayerInput player)
    {
        playersInStartZone.Add(player);
    }

    public void PlayerLeaveStartZone(PlayerInput player)
    {
        if(playersInStartZone.Contains(player))
        {
            playersInStartZone.Remove(player);
        }
    }


}
