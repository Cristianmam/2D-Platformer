using UnityEngine;

public class StaticTrap : MonoBehaviour
{
    [SerializeField]
    private int trapDamage;

    [SerializeField]
    private bool hasKnockback = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            if(hasKnockback)
                collision.GetComponent<PlayerManager>().TakeDamage(transform.position, trapDamage);
            else
                collision.GetComponent<PlayerManager>().TakeDamage(trapDamage);
    }
}
