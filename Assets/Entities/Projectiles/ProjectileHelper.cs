using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHelper : MonoBehaviour
{
    Projectile parentProjectile;
    private void Awake()
    {
        parentProjectile = gameObject.transform.GetComponentInParent<Projectile>();
    }

    public void DeactivateProjectile()
    {
        parentProjectile.DeactivateProjectile();
    }
}
