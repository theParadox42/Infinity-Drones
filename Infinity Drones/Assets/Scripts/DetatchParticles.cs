﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetatchParticles : MonoBehaviour
{
    ParticleSystem myParticleSystem;

    void Start () {
        myParticleSystem = GetComponent<ParticleSystem>();

    }
    void Update () {
        if (!myParticleSystem.IsAlive()) {
            if (myParticleSystem.particleCount <= 0) {
                Destroy(gameObject);
            }
        }
    }
    public void Detatch() {
        transform.parent = null;
        myParticleSystem.Stop();
    }
}
