using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 Direction { get; private set; }

    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float maxSpeedReduction = .9f;
    [SerializeField]
    private float speedReductionPerItem = 0.15f;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ParticleSystem stunParticles;

    [SerializeField]
    private SpriteRenderer sr;

    private new Rigidbody2D rigidbody;

    [SerializeField]
    private Transform throwStart;

    [SerializeField]
    private GameObject carrotProjectile;

    [Header("Stun Immunity")]

    [SerializeField]
    private float stunImmuneTime;

    [SerializeField]
    private AnimationCurve stunImmuneAnimation;


    public bool IsStun => stun > 0.0f;
    public bool IsStunImmune => stunImmune;
    public bool IsInsideSafeZone => insideSafeZone;

    private bool throwing = false;
    private float stun = 0.0f;
    private bool stunImmune;
    private bool siaHide;
    private float siaHideTime, siaMaxHideTime;
    private bool insideSafeZone = false;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Direction = ctx.action.ReadValue<Vector2>();
    }

    public void OnDrop(InputAction.CallbackContext ctx)
    {
        if (insideSafeZone) return;
        if (!ctx.action.triggered) return;

        player.Inventory.DropLastItem();
    }

    public void OnThrowController(InputAction.CallbackContext ctx)
    {
        if (player.Inventory.Items.Count == 0) return;
        if (insideSafeZone) return;

        Vector2 throw_direction = ctx.action.ReadValue<Vector2>();

        if (throw_direction.magnitude >= 1.0f)
        {
            if (!throwing)
            {
                throwing = true;
                throw_direction.Normalize();

                SpawnProjectile(throw_direction);
            }
        }
        else if (throw_direction.magnitude <= 0.3f)
        {
            throwing = false;
        }
    }

    public void OnThrowMouse(InputAction.CallbackContext ctx)
    {
        if (player.Inventory.Items.Count == 0) return;
        if (insideSafeZone) return;

        if (!ctx.action.triggered) return;

        Vector2 mouse_pos = Mouse.current.position.ReadValue();
        mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

        Vector2 throw_direction = mouse_pos - (Vector2)transform.position;
        throw_direction.Normalize();

        SpawnProjectile(throw_direction);
    }

    private void SpawnProjectile(Vector2 direction)
    {
        GameObject projectile = GameObject.Instantiate(carrotProjectile);
        projectile.transform.position = throwStart.position;
        projectile.GetComponent<CarrotProjectile>().Initialize(direction, gameObject);

        player.Inventory.RemoveItem(player.Inventory.Items[0]);
        player.Inventory.UpdateItemsPositions();
    }

    public void Stun(float stunTime)
    {
        if (stunImmune) return;

        stun = stunTime;

        var particleMain = stunParticles.main;
        particleMain.startLifetime = stunTime;
        stunParticles.Play();

        StartCoroutine(StunImmunity());

        //  drop all items
        while (player.Inventory.ItemsCount > 0)
            player.Inventory.DropLastItem();
    }

    void Update()
    {
        if (stunImmune && siaHideTime > 0.0f)
        {
            Color color = sr.color;
            color.a = Mathf.Lerp( color.a, siaHide ? 1.0f : 0.0f, (siaMaxHideTime - siaHideTime) / siaMaxHideTime );
            sr.color = color;
            siaHideTime -= Time.deltaTime;
        }

        //  get move speed
        float speed = moveSpeed - moveSpeed * Mathf.Min(maxSpeedReduction, player.Inventory.ItemsCount * speedReductionPerItem);

        //  stun
        if (stun > 0.0f)
        {
            stun -= Time.deltaTime;
        }
        //  move
        else
        {
            rigidbody.MovePosition(rigidbody.position + Direction * speed);
        }

        //  animation
        animator.SetBool("IsWalking", Direction != Vector2.zero && stun <= 0.0f);
        animator.SetFloat("AnimSpeed", moveSpeed / speed);
    }

    private IEnumerator StunImmunity()
    {
        stunImmune = true;

        float timeImmuned = 0.0f;
        while (timeImmuned < stunImmuneTime)
        {
            siaHide = !siaHide;
            float transitionTime = stunImmuneAnimation.Evaluate(timeImmuned / stunImmuneTime);
            if (transitionTime + timeImmuned > stunImmuneTime)
            {
                transitionTime = stunImmuneTime - timeImmuned;
            }

            siaHideTime = siaMaxHideTime = transitionTime;
            //print(transitionTime);
            timeImmuned += transitionTime;
            yield return new WaitForSeconds(transitionTime);
        }
        siaHide = false;

        stunImmune = false;
    }


    public void SetInsideSafeZone(bool value)
    {
        insideSafeZone = value;
    }
}
