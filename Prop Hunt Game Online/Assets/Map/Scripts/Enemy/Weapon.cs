using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> muzzleFlashes = new List<ParticleSystem>();

    public void PlayMuzzleFlash()
    {
        foreach (ParticleSystem muzzleFlash in muzzleFlashes)
            muzzleFlash.Play();
    }
}
