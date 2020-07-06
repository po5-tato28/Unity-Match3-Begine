using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// plays UI effects when player reaches scoring goal
public class ScoreStar : MonoBehaviour
{
    // reference to the icon 
    public Image m_star;

    // reference to activation particle effect
    public ParticlePlayer m_starFX;

    // delay between particles and turning icon on
    public float m_delay = 0.5f;

    // activation sound clip
    public AudioClip m_starSound;

    // have we been activated already?
    public bool m_activated = false;


    void Start()
    {
        SetActive(false);
    }

    // turn the icon on or off
    public void SetActive(bool state)
    {
        if (m_star != null)
        {
            m_star.gameObject.SetActive(state);
        }
    }

    // activate the star
    public void Activate()
    {
        // only activate once
        if (m_activated)
        {
            return;
        }

        // invoke ActivateRoutine coroutine
        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        // we are activated
        m_activated = true;

        // play the ParticlePlayer
        if (m_starFX != null)
        {
            m_starFX.Play();
        }

        // play the starSound
        if (SoundManager.Instance != null && m_starSound != null)
        {
            SoundManager.Instance.PlayClipAtPoint(m_starSound, Vector3.zero, SoundManager.Instance.m_fxVolume);
        }

        yield return YieldInstructionCache.WaitForSeconds(m_delay);

        // turn on the icon
        SetActive(true);
    }

}
