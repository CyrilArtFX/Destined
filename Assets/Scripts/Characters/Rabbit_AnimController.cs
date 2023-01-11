using UnityEngine;

public class Rabbit_AnimController : MonoBehaviour
{
    public bool CanMove = false;

    public void AllowMove() { CanMove = true; }
    public void DisallowMove() { CanMove = false; }
}