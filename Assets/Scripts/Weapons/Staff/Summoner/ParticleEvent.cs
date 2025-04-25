using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : MonoBehaviour
{
    public ParticleSystem particles;

    public void PlayParticles()
    {
        particles.Play();
    }
}
