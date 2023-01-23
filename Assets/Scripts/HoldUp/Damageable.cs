using UnityEngine;

namespace HoldUp
{
    public enum DeathMode
    {
        Destroy,
        Respawnable
    }

    public class Damageable : MonoBehaviour
    {
        [SerializeField]
        private Lifebar lifebar;

        [SerializeField]
        private float maxLife;

        [SerializeField]
        private DeathMode deathMode;

        private float life;

        void Start()
        {
            life = maxLife;
            lifebar.Initialize(maxLife);
        }

        public void DealDamages(float damages)
        {
            life = Mathf.Max(life - damages, 0.0f);
            lifebar.ChangeLife(life);

            if(life == 0)
            {
                Death();
            }
        }

        private void Death()
        {
            if(deathMode == DeathMode.Destroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
