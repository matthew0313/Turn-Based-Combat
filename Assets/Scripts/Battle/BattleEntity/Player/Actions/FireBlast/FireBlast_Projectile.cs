using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Player_Battle
{
    public class FireBlast_Projectile : PooledPrefab<FireBlast_Projectile>
    {
        [SerializeField] PooledParticle explodeParticle;
        public void Explode()
        {
            explodeParticle.Get(transform.position, Quaternion.identity);
            Release();
        }
    }
}