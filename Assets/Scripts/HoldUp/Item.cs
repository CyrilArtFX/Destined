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
    }
}
