using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Player Controller")]
    public class PlayerController : Core.Players.PlayerController
    {
        [SerializeField]
        private Lifebar lifebar;

        void Start()
        {
            lifebar.Initialize(15.0f);
        }

        public void OnDebugAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.action.triggered) return;

            lifebar.LoseLife(1.0f);
        }

        void Update()
        {
            //  move
            if (!InCinematic)
            {
                rigidbody.MovePosition(rigidbody.position + Direction * moveSpeed);
            }

            //  animation
            animator.SetBool("IsWalking", Direction != Vector2.zero && !InCinematic);
            animator.SetFloat("AnimSpeed", moveSpeed);
        }
    }
}
