using UnityEngine;

namespace HoldUp
{
    public class Lifebar : MonoBehaviour
    {
        [SerializeField]
        private Transform lifebarRect;
        private float fullScale;

        [SerializeField]
        private Gradient colorEvolution;

        private float life, maxLife;

        [SerializeField]
        private SpriteRenderer barRenderer;
        [SerializeField]
        private SpriteRenderer backRenderer;

        [SerializeField]
        private bool hideOnFullLife, hideOnZeroLife;


        public void Initialize(float maxLife_)
        {
            life = maxLife = maxLife_;
            fullScale = lifebarRect.localScale.x;
            UpdateBar();
        }

        public void LoseLife(float lifeLost)
        {
            life = Mathf.Max(life - lifeLost, 0.0f);
            UpdateBar();
        }

        public void GainLife(float lifeGained)
        {
            life = Mathf.Min(life + lifeGained, maxLife);
            UpdateBar();
        }

        private void UpdateBar()
        {
            float lifeFraction = life / maxLife;
            lifebarRect.localScale = new Vector3(lifeFraction * fullScale, 1.0f, 1.0f);
            barRenderer.color = colorEvolution.Evaluate(lifeFraction);

            ShowRenderers();
            if (hideOnFullLife)
            {
                if (life == maxLife)
                {
                    HideRenderers();
                }
            }
            if (hideOnZeroLife)
            {
                if (life == 0.0f)
                {
                    HideRenderers();
                }
            }

        }

        private void ShowRenderers()
        {
            barRenderer.enabled = true;
            backRenderer.enabled = true;
        }

        private void HideRenderers()
        {
            barRenderer.enabled = false;
            backRenderer.enabled = false;
        }
    }
}
