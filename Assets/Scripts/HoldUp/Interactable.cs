using UnityEngine;
using UnityEngine.Events;

namespace HoldUp
{
    public class Interactable : MonoBehaviour
    {
        [HideInInspector]
        public bool InteractionPossible;

        public UnityEvent PressInteraction;
        public UnityEvent ReleaseInteraction;

        public string InteractionDisplay;
    }
}
