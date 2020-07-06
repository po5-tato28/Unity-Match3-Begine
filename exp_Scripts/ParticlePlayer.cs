using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour {

    public ParticleSystem[] m_allParticles;
    public float m_lifetime = 1f;
    public bool m_destroyImmediately = true;


    void Start() {
        m_allParticles = GetComponentsInChildren<ParticleSystem>();

        if (m_destroyImmediately) { 
            Destroy(gameObject, m_lifetime);
        }
    }

    public void Play() {
        foreach (ParticleSystem ps in m_allParticles) {
            ps.Stop();
            ps.Play();
        }
        //Destroy(gameObject, m_lifetime);
    }
}
