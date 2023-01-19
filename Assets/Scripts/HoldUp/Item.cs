using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldUp
{
    public abstract class Item : MonoBehaviour
    {
        protected PlayerController playerController;

        public virtual void Initialize(PlayerController controller)
        {
            playerController = controller;
        }

        public virtual void OnUse(InputAction.CallbackContext ctx)
        {
        }

        public virtual void OnAimJoystick(InputAction.CallbackContext ctx)
        {
        }

        public virtual void OnAimMouse(InputAction.CallbackContext ctx)
        {
        }

        protected void Rotate(Vector2 rotationDirection)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg));
        }
    }
}
