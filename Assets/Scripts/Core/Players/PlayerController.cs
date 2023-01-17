using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Players
{
    public abstract class PlayerController : MonoBehaviour
    {
        public Vector2 Direction { get; private set; }
        public bool InCinematic { get; protected set; }
        public bool InsideSafeZone { get; protected set; }

        [SerializeField]
        protected float moveSpeed = 5.0f;
        [SerializeField]
        protected Player player;
        [SerializeField]
        protected Animator animator;

        [SerializeField]
        protected new SpriteRenderer renderer;
        protected new Rigidbody2D rigidbody;


        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            Direction = ctx.action.ReadValue<Vector2>();
        }

        public void SetInsideSafeZone(bool value)
        {
            InsideSafeZone = value;
        }

        public void SetInCinematic(bool value)
        {
            InCinematic = value;
        }

        public virtual void ClearEffects()
        {
            InsideSafeZone = false;
        }
    }
}
