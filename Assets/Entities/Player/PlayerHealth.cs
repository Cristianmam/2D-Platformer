using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public enum HealthStatus
    {
        Normal = 0,
        Invulnerable = 1,
        IgnoreDamage = 2
    }
    public HealthStatus status { get; private set; }

    [SerializeField]
    private int maxHealth = 3;

    public int currentHealth;

    [SerializeField]
    private float invulnerabilityTime = 2;

    private float invulTimer = 0;

    [SerializeField]
    private float flashInterval = 0.5f;
    private float flashTimer = 0;

    private bool flashed = false;

    [SerializeField]
    private Color flashColor;

    private GameController gameController;

    private SpriteRenderer spriteRenderer;

    private PlayerManager playerManager;

    public void InitializeHealth( PlayerManager pm, GameController gc, SpriteRenderer sr)
    {
        gameController = gc;
        playerManager = pm;
        spriteRenderer = sr;

        invulTimer = invulnerabilityTime;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(status == HealthStatus.Invulnerable)
        {
            if (invulTimer < invulnerabilityTime)
            {
                invulTimer += Time.deltaTime;

                FlashPlayer();
            }
            else
            {
                if (status != HealthStatus.IgnoreDamage)
                    status = HealthStatus.Normal;

                if (flashed)
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }


        
    }

    private void FlashPlayer()
    {
        if (flashTimer < flashInterval)
            flashTimer += Time.deltaTime;
        else
        {
            if (flashed)
            {
                spriteRenderer.color = Color.white;
                flashed = false;
            }
            else
            {
                spriteRenderer.color = flashColor;
                flashed = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (status == HealthStatus.Invulnerable || status == HealthStatus.IgnoreDamage)
            return;

        //This could be removed if we wanted to do stuff with negative health
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        if (currentHealth == 0)
            //Tell the player manager we died
            return;

        invulTimer = 0;
        flashTimer = 0;
        status = HealthStatus.Invulnerable;

        //Consider knocking the player backwards with a function in PlayerMovement
    }

    public void GainHealth(int health)
    {
        //This could be removed if we wanted to do stuff with overhealing
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
    }
}
