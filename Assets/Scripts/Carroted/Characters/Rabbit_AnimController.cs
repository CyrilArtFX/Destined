using System;
using UnityEngine;
using UnityEngine.Events;

namespace Carroted
{
    public class Rabbit_AnimController : MonoBehaviour
    {
        public bool CanMove = false;
        public bool IsChargeJumping = false;

        public Action OnAttack;
        public Action OnChargeJumpEnd;

        public void AllowMove() => CanMove = true;
        public void DisallowMove() => CanMove = false;

        public void EnableChargeJumping() => IsChargeJumping = true;
        public void DisableChargeJumping() => IsChargeJumping = false;

        public void InvokeOnAttack() => OnAttack.Invoke();
        public void InvokeOnChargeJumpEnd() => OnChargeJumpEnd.Invoke();
    }
}