using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Player Controller")]
    public class PlayerController : Core.Players.PlayerController
    {
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
