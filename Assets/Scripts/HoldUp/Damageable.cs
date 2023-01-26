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
        [Header("Life Properties")]
        [SerializeField]
        private float maxLife;
        [SerializeField]
        private DeathMode deathMode;

        [Header("Health Regeneration")]
        [SerializeField]
        private bool regenHealth;
        [SerializeField]
        private float regenPerSecond;
        [SerializeField, Tooltip("The time the player needs to have no damage for the regen to start")]
        private float timeForTriggerRegen;


        [Header("Components References")]
        [SerializeField]
        private Lifebar lifebar;

        [SerializeField]
        private GameObject damageParticlesPrefab;

        [SerializeField, Tooltip("Only fill if this script is on a player")]
        private PlayerController playerController;

        private float life;
        private float timeRemainingForRegen;

        void Start()
        {
            life = maxLife;
            lifebar.Initialize(maxLife);
        }

        public void DealDamages(float damages, Vector3 origin)
        {
            life = Mathf.Max(life - damages, 0.0f);
            lifebar.ChangeLife(life);

            if (life == 0)
            {
                Death();
            }

            //  play damage particles
            if (damageParticlesPrefab != null)
            {
                ParticleSystem particles = Instantiate(damageParticlesPrefab, GameManager.instance.transform).GetComponent<ParticleSystem>();
                particles.transform.position = transform.position;
                particles.transform.eulerAngles = Vector2Utils.GetDirectionAngles(transform.position - origin);
                particles.Emit((int)(damages * 3.0f));
            }

            if (regenHealth)
                timeRemainingForRegen = timeForTriggerRegen;
        }

        void Update()
        {
            if(regenHealth && life > 0.0f && life < maxLife)
            {
                if (timeRemainingForRegen > 0.0f)
                {
                    timeRemainingForRegen = Mathf.Max(timeRemainingForRegen - Time.deltaTime, 0.0f);
                }

                if(timeRemainingForRegen == 0.0f)
                {
                    life = Mathf.Min(life + regenPerSecond * Time.deltaTime, maxLife);
                    lifebar.ChangeLife(life);
                }
            }
        }

        public void ResetLife()
        {
            life = maxLife;
            lifebar.ChangeLife(life);
        }

        private void Death()
        {
            switch (deathMode)
            {
                case DeathMode.Destroy:
                    Destroy(gameObject);
                    break;
                case DeathMode.Respawnable:
                    if (playerController && !playerController.Dead)
                    {
                        playerController.SetDead();
                    }
                    break;
            }
        }
    }
}
