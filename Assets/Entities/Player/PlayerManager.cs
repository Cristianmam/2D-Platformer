using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameController gameController { get; private set; }

    public PlayerMovement movement { get; private set; }
    public PlayerHealth health { get; private set; }
    public Animator animator { get; private set; }

    public SpriteRenderer spriteRenderer { get; private set; }


    public void InitializePlayer(GameController gc)
    {
        gameController = gc;

        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement.InitializeMovement(this, gameController, animator);
        health.InitializeHealth(this, gameController, spriteRenderer);

    }

    public void TakeDamage(Vector3 triggerOrigin, int damage)
    {
        TakeDamage(damage);
        //push the player back based on the collision point
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }
}
