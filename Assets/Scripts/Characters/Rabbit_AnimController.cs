using System;
using UnityEngine;
using UnityEngine.Events;

public class Rabbit_AnimController : MonoBehaviour
{
    public bool CanMove = false;

    public Action OnAnimAttack;

    public void AllowMove() => CanMove = true;
    public void DisallowMove() => CanMove = false;

    public void InvokeAttackEvent() => OnAnimAttack.Invoke();
}