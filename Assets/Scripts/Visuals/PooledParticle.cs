using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PooledParticle : PooledPrefab<PooledParticle>
{
    [SerializeField] List<ParticleSystem> particles;
    protected override void OnGet()
    {
        base.OnGet();
        foreach (var i in particles) i.Play();
    }
    private void Update()
    {
        foreach(var i in particles)
        {
            if (i.isPlaying) return;
        }
        Release();
    }
}