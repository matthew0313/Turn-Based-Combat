using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public class WaterBlast_Projectile : PooledPrefab<WaterBlast_Projectile>
    {
        [SerializeField] PooledEffect explodeParticle;
        public void Explode()
        {
            explodeParticle.Get(transform.position, Quaternion.identity);
            Release();
        }
    }
}