using UnityEngine;
using UnityEngine.InputSystem;
using Core;

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
                mover.Move(Direction);
            }
        }
    }
}
