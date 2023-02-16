using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //TODO move all key references to player manager or a struct referenced there to prepare for key rebinds

    PlayerManager playerManager;

    GameController gameController;

    Animator animator;

    [SerializeField]
    private Transform upFirePoint;
    [SerializeField]
    private Transform rightFirePoint;
    [SerializeField]
    private Transform downFirePoint;

    //Could be expanded for more ranged attacks
    [SerializeField]
    private Projectile projectilePrefab;

    private Dictionary<Projectile,List<Projectile>> projectilePool = new Dictionary<Projectile, List<Projectile>>();
    private Dictionary<Projectile,GameObject> projectileHolders = new Dictionary<Projectile, GameObject>();

    [SerializeField]
    private int meleeAttackDamage = 1;

    private Transform playerProjectilePools;


    public void InitializeAttack(PlayerManager pm, GameController gc, Animator anim)
    {
        playerManager = pm;
        gameController = gc;
        animator = anim;

        playerProjectilePools = new GameObject("Player projectile pools").transform;
        playerProjectilePools.position = gc.transform.position;
        playerProjectilePools.transform.parent = gc.transform;

        projectilePool[projectilePrefab] = new List<Projectile>();
        projectilePool[projectilePrefab].Add(CreateNewInstaceOfProjectile(projectilePrefab));
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.gameplayActive || playerManager.playerState != PlayerManager.PlayerStates.Normal)
            return;


        //Add a cooldown
        if (Input.GetKeyDown(KeyCode.F))
        {
            FireProjectile(projectilePrefab);
        }
    }

    private Projectile GetProjectile(Projectile type)
    {
        if (projectilePool.ContainsKey(type))
        {
            //If we have a pool of the type, check for any inactive projectiles
            foreach (Projectile p in projectilePool[type])
            {
                if (!p.gameObject.activeInHierarchy)
                {
                    p.gameObject.SetActive(true);
                    return p;
                }   
            }
        }
        //If we dont have such key in the dictionary or there is no inactive projectile just create a new instance of the projectile and add it
        Projectile temp = CreateNewInstaceOfProjectile(type);
        projectilePool[type].Add(temp);
        return temp;
    }

    private Projectile CreateNewInstaceOfProjectile (Projectile type)
    {
        Projectile tempProjectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);

        if(projectileHolders.TryGetValue(type,out GameObject holder))
        {
            tempProjectile.transform.parent = holder.transform;
        }
        else
        {
            GameObject newHolder = new GameObject($"{type.gameObject.name} holder");
            newHolder.transform.position = playerManager.transform.position;
            newHolder.transform.parent = playerProjectilePools;
            tempProjectile.transform.parent = newHolder.transform;
            projectileHolders[type] = newHolder;
        }

        tempProjectile.gameObject.SetActive(false);

        return tempProjectile;
    }

    private void FireProjectile(Projectile type)
    {
        Projectile projectile;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            projectile = GetProjectile(type);
            projectile.Fire(upFirePoint.position, 90);
            return;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) && !playerManager.movement.isGrounded)
        {
            projectile = GetProjectile(type);
            projectile.Fire(upFirePoint.position, 270);
            return;
        }

        if(Mathf.Sign(transform.localScale.x) > 0)
        {
            projectile = GetProjectile(type);
            projectile.Fire(rightFirePoint.position, 0);
            return;
        }
        else
        {
            projectile = GetProjectile(type);
            projectile.Fire(rightFirePoint.position, 180);
            return;
        }

    }
}
