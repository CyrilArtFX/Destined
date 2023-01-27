using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoldUp
{
    public class Van : MonoBehaviour
    {
        public SpriteRenderer[] CharactersRenderers => charactersRenderers;

        [SerializeField]
        private SpriteRenderer[] charactersRenderers;
        /*[SerializeField]
        private SpriteRenderer[] doorsRenderers;*/

        private Animator animator;

        private bool isAnimFinished = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public IEnumerator CoroutineStartAnimation()
        {
            isAnimFinished = false;
            animator.SetBool("IsStarting", true);

            yield return new WaitUntil(() => isAnimFinished);

            animator.SetBool("IsStarting", false);
        }

        public void FinishAnimation() => isAnimFinished = true;
    }
}
