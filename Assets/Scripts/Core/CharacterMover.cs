using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMover : MonoBehaviour
    {
        public Vector2 Direction { get; private set; }
        public float AnimSpeed { get; private set; }
        public bool HasMoveThisFrame { get; private set; }

        public float MoveSpeed = 1.0f;

        [SerializeField]
        private Animator animator;

        private new Rigidbody2D rigidbody;

        void Awake()
        {
            AnimSpeed = 1.0f;

            rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 dir, float speed_multiplier = 1.0f, float anim_speed = 1.0f)
        {
            float speed = MoveSpeed * speed_multiplier;

            dir.Normalize();

            rigidbody.MovePosition(rigidbody.position + dir * speed);

            Direction = dir;
            AnimSpeed = anim_speed;
            HasMoveThisFrame = true;
        }

        void Update()
        {
            //  animation
            animator.SetBool("IsWalking", Direction != Vector2.zero);
            animator.SetFloat("AnimSpeed", AnimSpeed);

            //  reset moving direction next frame
            if ( !HasMoveThisFrame )
            {
                Direction = Vector2.zero;
                AnimSpeed = 1.0f;
            }
            HasMoveThisFrame = false;
        }
    }
}
