using Core.Items;
using Core.Players;
using System.Collections.Generic;
using UnityEngine;

namespace Carroted
{
    public class BadRabbit : MonoBehaviour
    {
        enum State
        {
            PATROL,
            ATTACK,
        }

        [Header("Behaviour")]
        [SerializeField]
        private float moveSpeed = .1f;
        [SerializeField]
        private float chaseSpeed = .15f;
        [SerializeField]
        private Transform territoryCenter;
        [SerializeField]
        private float territoryRadius = 5.0f;
        [SerializeField]
        private float moveAcceptanceRadius = 0.1f;
        [SerializeField]
        private Vector2 idleTimeRange = new(0.8f, 1.2f);
        [SerializeField]
        private float stunTime = 1.0f;
        [SerializeField]
        private float attackRadius = 0.075f;
        [SerializeField]
        private float searchRadius = 2.0f;
        [SerializeField]
        private float searchPercent = 0.4f;

        [Header("Charge Attack")]
        [SerializeField]
        private float chargeMoveSpeed = .3f;
        [SerializeField]
        private float chargeStunTime = 1.0f;
        [SerializeField]
        private float chargeRadius = 3.5f;
        [SerializeField]
        private float chargeCooldown = 2.0f;
        [SerializeField]
        private ParticleSystem chargeParticles;

        [Header("Gold Carrot")]
        [SerializeField]
        private GameObject goldCarrotPrefab;
        [SerializeField]
        private float chancePerCarrot = 0.1f;

        [Header("References")]
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Rabbit_AnimController animController;
        [SerializeField]
        private Inventory inventory;

        private Player playerTarget;
        private Vector3 moveTarget;
        private Vector3 moveDir;
        private bool isIdling = true;
        private bool isTargetLocked = false;
        private float currentIdleTime = 0.0f;
        private float currentChargeCooldown = 0.0f;
        private State state = State.PATROL;
        private bool isNextDropGoldCarrot = false;

        void Start()
        {
            moveTarget = transform.position;
            currentIdleTime = idleTimeRange.x;

            animController.OnAttack = OnAnimAttack;
            animController.OnChargeJumpEnd = OnAnimChargeJumpEnd;

            GameEvents.OnCollect.AddListener(GameEventOnCollect);

            ParticleSystem.ShapeModule shape = chargeParticles.shape;
            shape.radius = chargeRadius;
        }

        void Update()
        {
            switch (state)
            {
                case State.ATTACK:
                    if (playerTarget != null && !playerTarget.Controller.InsideSafeZone)
                        moveTarget = playerTarget.transform.position;
                    else
                    {
                        SetPlayerTarget(null);
                        isTargetLocked = false;
                    }

                    //  decrease charge cooldown
                    if (!animController.IsChargeJumping)
                        currentChargeCooldown -= Time.deltaTime;

                    break;
            }

            //  movement
            Vector3 dir = moveTarget - transform.position;
            if (!isIdling && animController.CanMove)
            {
                float speed = Time.deltaTime;
                if (state == State.ATTACK)
                    speed *= animController.IsChargeJumping ? chargeMoveSpeed : chaseSpeed;
                else
                    speed *= moveSpeed;

                transform.position = transform.position + speed * moveDir;
            }
            //  destination reached
            else if (dir.magnitude <= moveAcceptanceRadius)
            {
                OnMoveUpdateEnd();
            }
            //  get next movement direction
            else
            {
                moveDir = dir.normalized;

                switch (state)
                {
                    case State.ATTACK:
                        if ((playerTarget.transform.position - transform.position).sqrMagnitude > chargeRadius * chargeRadius)
                        {
                            if (currentChargeCooldown <= 0.0f)
                            {
                                animator.SetBool("IsChargeJumping", true);
                                currentChargeCooldown = chargeCooldown;
                            }
                        }
                        break;
                }
            }

            animator.SetBool("IsWalking", !isIdling);
            animator.SetFloat("AnimSpeed", state == State.ATTACK ? chaseSpeed / moveSpeed : 1.0f);
        }

        void SetPlayerTarget(Player player)
        {
            playerTarget = player;

            //  change state
            if (playerTarget != null)
                state = State.ATTACK;
            else
            {
                state = State.PATROL;
                moveTarget = FindNextPatrolPosition();
            }

            //  reset idle
            ResetIdle();
        }

        void TryChangePriorityPlayer(Player player)
        {
            if (isTargetLocked || isNextDropGoldCarrot) return;

            PlayerController controller = player.Controller as PlayerController;
            if (controller.IsStun || controller.IsStunImmune || controller.InsideSafeZone) return;

            //  check priority w/ current player 
            if (playerTarget != null && playerTarget != player)
            {
                //  priority on the nearest
                float current_dist_sqr = (player.transform.position - playerTarget.transform.position).sqrMagnitude;
                if ((player.transform.position - transform.position).sqrMagnitude > current_dist_sqr)
                    return;
            }

            //  set new target
            SetPlayerTarget(player);
        }

        void ResetIdle()
        {
            isIdling = false;
            currentIdleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        }

        void OnAnimAttack()
        {
            //  damage player
            if (playerTarget != null)
            {
                //  stun target
                (playerTarget.Controller as PlayerController).Stun( stunTime );

                //  stop chasing player
                SetPlayerTarget(null);

                SearchCarrots();
            }

            //  idling
            isIdling = false;
            isTargetLocked = false;
            animator.ResetTrigger("Attack");
        }

        void OnAnimChargeJumpEnd()
        {
            //  stun in circle
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, chargeRadius))
            {
                if (!collider.TryGetComponent(out PlayerController controller)) continue;

                controller.Stun(chargeStunTime);
            }

            //  play particles
            chargeParticles.Play();

            //  stop chasing player
            SetPlayerTarget(null);

            SearchCarrots();

            //  idling
            isIdling = false;
            animator.SetBool("IsChargeJumping", false);
        }

        void SearchCarrots()
        {
            if (inventory.ItemsCount > inventory.MaxCount) return;

            //  search for collectibles
            List<Collectible> found_items = new();
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, searchRadius))
            {
                if (!collider.TryGetComponent(out Carrot item)) continue;
                if (item.Type == Carrot.ItemType.GOLD_CARROT) continue;

                found_items.Add(item);
            }

            //  grab a percent of them
            int grab_amount = (int)Mathf.Floor(found_items.Count * searchPercent);
            for (int i = 0; i < grab_amount; i++)
            {
                if (inventory.ItemsCount > inventory.MaxCount) return;

                inventory.AddItem(found_items[i]);
            }

            //  check for full inventory
            if (inventory.ItemsCount == inventory.MaxCount)
            {
                //  move away
                SetPlayerTarget(null);

                //  ensure a gold carrot drop
                isNextDropGoldCarrot = true;
            }
        }

        void OnMoveUpdateEnd()
        {
            switch (state)
            {
                //  attack player
                case State.ATTACK:
                    if (playerTarget != null && !isIdling)
                    {
                        if ((transform.position - playerTarget.transform.position).sqrMagnitude <= attackRadius * attackRadius)
                        {
                            isIdling = true;
                            isTargetLocked = true;
                            animator.SetTrigger("Attack");
                        }
                    }
                    break;
                //  idling after movement
                case State.PATROL:
                    isIdling = true;

                    if ((currentIdleTime -= Time.deltaTime) <= 0.0f)
                    {
                        moveTarget = FindNextPatrolPosition();
                        ResetIdle();

                        //  dropping items
                        if (inventory.ItemsCount > 0)
                        {
                            //  list non-gold carrots
                            List<Carrot> non_gold_carrots = new();
                            foreach (Carrot item in inventory.Items)
                            {
                                if (item.Type == Carrot.ItemType.GOLD_CARROT) continue;
                                non_gold_carrots.Add(item);
                            }

                            //  chance to craft a gold carrot instead
                            float chance = chancePerCarrot * non_gold_carrots.Count;
                            if (isNextDropGoldCarrot || Random.Range(0.0f, 1.0f) <= chance)
                            {
                                //  instance gold carrot
                                GameObject gold_carrot = Instantiate(goldCarrotPrefab, GameManager.instance.transform);
                                gold_carrot.transform.position = transform.position;

                                //  clear inventory
                                foreach (Collectible item in non_gold_carrots)
                                    inventory.RemoveItem(item);
                                inventory.UpdateItemsPositions();

                                isNextDropGoldCarrot = false;
                            }
                            //  drop items
                            else
                            {
                                int drop_amount = Random.Range(1, 2);
                                for (int i = 0; i <= drop_amount; i++)
                                    inventory.DropLastItem();
                            }
                        }
                    }
                    break;
            }
        }

        Vector3 FindNextPatrolPosition()
        {
            Vector3 pos = Vector3.zero;

            for (int iter = 0; iter < 3; iter++)
            {
                pos = territoryCenter.position;

                //  get a random position in circle
                float ang = Random.Range(0.0f, 360.0f);
                float radius = Random.Range(territoryRadius * 0.1f, territoryRadius);
                pos.x += Mathf.Cos(ang) * radius;
                pos.y += Mathf.Sin(ang) * radius;

                //  search for a new position if too close
                if ((pos - transform.position).magnitude > moveAcceptanceRadius * 2)
                    break;
            }

            return pos;
        }

        void GameEventOnCollect(Transform owner, Collectible item)
        {
            if (!owner.TryGetComponent(out Player player)) return;

            //  check is within territory
            float dist_sqr = (player.transform.position - territoryCenter.position).sqrMagnitude;
            if (dist_sqr > territoryRadius * territoryRadius)
                return;

            TryChangePriorityPlayer(player);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out Player player)) return;

            TryChangePriorityPlayer(player);
        }

        void OnDrawGizmos()
        {
            //  move target
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveTarget, moveAcceptanceRadius);

            //  attack radius
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            //  territory
            if (territoryCenter != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(territoryCenter.position, territoryRadius);
            }

            //  player target
            if (playerTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(playerTarget.transform.position, 1.0f);
            }

            //  charge
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chargeRadius);

            //  search
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }
    }
}
