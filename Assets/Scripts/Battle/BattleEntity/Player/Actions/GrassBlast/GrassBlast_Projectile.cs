using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public class GrassBlast_Projectile : PooledPrefab<GrassBlast_Projectile>
    {
        [SerializeField] PooledParticle explodeParticle;
        public void Explode()
        {
            explodeParticle.Get(transform.position, Quaternion.identity);
            Release();
        }
    }
}