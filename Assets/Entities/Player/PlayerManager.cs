using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameController gameController { get; private set; }

    public PlayerMovement movement { get; private set; }
    public PlayerHealth health { get; private set; }
    public PlayerAttack attack { get; private set; }
    public Animator animator { get; private set; }

    public SpriteRenderer spriteRenderer { get; private set; }

    public enum PlayerStates
    {
        Normal = 0,
        Attacking = 1
    }

    public PlayerStates playerState { get; private set; }

    public void InitializePlayer(GameController gc)
    {
        gameController = gc;

        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        attack = GetComponent<PlayerAttack>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement.InitializeMovement(this, gameController, animator);
        health.InitializeHealth(this, gameController, spriteRenderer);
        attack.InitializeAttack(this, gameController, animator);

        playerState = PlayerStates.Normal;
    }

    public void TakeDamage(Vector3 triggerOrigin, int damage)
    {
        if (health.status == PlayerHealth.HealthStatus.IgnoreDamage || health.status == PlayerHealth.HealthStatus.Invulnerable)
            return;

        TakeDamage(damage);
        movement.KnockPlayerBack(triggerOrigin);
    }

    public void TakeDamage(int damage)
    {
        if (health.status == PlayerHealth.HealthStatus.IgnoreDamage || health.status == PlayerHealth.HealthStatus.Invulnerable)
            return;

        if (damage != 0)
            health.TakeDamage(damage);
    }

    public void SetState(PlayerStates newstate)
    {
        playerState = newstate;
    }
}
