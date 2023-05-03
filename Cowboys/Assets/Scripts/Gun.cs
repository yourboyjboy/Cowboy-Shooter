using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private ParticleSystem gunshoteffect;
    // Start is called before the first frame update
    void Start()
    {
        gunshoteffect = GetComponentInChildren<ParticleSystem>();
    }

    void Shoot()
    {
        gunshoteffect.Play();
    }
}
