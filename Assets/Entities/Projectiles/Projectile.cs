using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed = 0.1f;

    [SerializeField]
    private bool playerProjectile = false;

    [SerializeField]
    private float lifespan;

    private float timeSpawned = 0;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private bool hasKnockback = true;

    Vector3 movementDirection;

    private bool stopped = true;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (timeSpawned < lifespan)
            timeSpawned += Time.deltaTime;
        else
            DeactivateProjectile();

        if (!stopped)
        {
            transform.position += movementDirection * projectileSpeed * Time.deltaTime;
        }
    }

    public void Fire(Vector3 startFrom, float angle)
    {
        transform.position = startFrom;

        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.Rotate(new Vector3(0, 0, angle));

        movementDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

        movementDirection = movementDirection.normalized;

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        stopped = false;
    }

    public void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTerrain(collision.gameObject.tag))
        {
            animator.SetTrigger("Hit");
            stopped = true;
            return;
        }
            

        if (playerProjectile)
        {
            if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "ShootableSwitch")
            {
                animator.SetTrigger("Hit");
                //Get the component as necesary and execute the "Damage" or "Hit" function

                stopped = true;
            }
        }
        else
        {
            if(collision.gameObject.tag == "Player")
            {
                animator.SetTrigger("Hit");
                //Get the component as necesary and execute the "Damage" or "Hit" function

                if (hasKnockback)
                    collision.GetComponent<PlayerManager>().TakeDamage(transform.position, damage);
                else
                    collision.GetComponent<PlayerManager>().TakeDamage(damage);
                stopped = true;
            }
        }
        
    }

    private bool IsTerrain(string value)
    {
        if (value == "Ground")
            return true;

        if (value == "Wall")
            return true;

        if (value == "Trap")
            return true;

        if (value == "OneWayPlatform")
            return true;

        return false;
    }
}
