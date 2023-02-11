using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerManager playerManager;

    GameController gameController;

    Animator animator;

    [SerializeField]
    private Projectile projectilePrefab;

    private List<Projectile> projectilePool;

    [SerializeField]
    private int meleeAttackDamage = 1;


    public void InitializeAttack(PlayerManager pm, GameController gc, Animator anim)
    {
        playerManager = pm;
        gameController = gc;
        animator = anim;

        //Initialize the projectile prefab and the projectile pool
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
