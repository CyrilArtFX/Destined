using UnityEngine;
using Utility;

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
        private GameObject damageParticlesPrefab;

        [SerializeField]
        private DeathMode deathMode;

        [SerializeField, Tooltip("Only fill if this script is on a player")]
        private PlayerController playerController;

        private float life;

        void Start()
        {
            life = maxLife;
            lifebar.Initialize(maxLife);
        }

        public void DealDamages(float damages, Vector3 origin)
        {
            life = Mathf.Max(life - damages, 0.0f);
            lifebar.ChangeLife(life);

            if(life == 0)
            {
                Death();
            }

            //  play damage particles
            if (damageParticlesPrefab != null)
            {
                ParticleSystem particles = Instantiate(damageParticlesPrefab, GameManager.instance.transform).GetComponent<ParticleSystem>();
                particles.transform.position = transform.position;
                particles.transform.eulerAngles = Vector2Utils.GetDirectionAngles(transform.position - origin);
                particles.Emit( (int) (damages * 3.0f) );
            }
        }

        public void ResetLife()
        {
            life = maxLife;
            lifebar.ChangeLife(life);
        }

        private void Death()
        {
            switch(deathMode)
            {
                case DeathMode.Destroy:
                    Destroy(gameObject);
                    break;
                case DeathMode.Respawnable:
                    if(playerController && !playerController.Dead)
                    {
                        playerController.SetDead();
                    }
                    break;
            }
        }
    }
}
