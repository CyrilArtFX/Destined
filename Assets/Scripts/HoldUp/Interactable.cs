using UnityEngine;
using UnityEngine.Events;

namespace HoldUp
{
    public class Interactable : MonoBehaviour
    {
        public bool InteractionPossible;

        public UnityEvent<PlayerController> PressInteraction;
        public UnityEvent<PlayerController> ReleaseInteraction;
        public UnityEvent<PlayerController> ExitZoneInteraction;

        public string InteractionDisplay;
    }
}
