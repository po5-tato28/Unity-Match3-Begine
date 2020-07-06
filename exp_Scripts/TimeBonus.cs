using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePiece))]
public class TimeBonus : MonoBehaviour
{
    [Range(0, 5)]
    public int m_bonusValue = 5;

    [Range(0f, 1f)]
    public float m_chanceForBonus = 0.1f;

    public GameObject m_bonusGlow;
    public Material[] m_bonusMaterials;

    public GameObject m_ringGlow;


    void Start() {
        // generate a random number to check against chance for bonus
        float random = Random.Range(0f, 1f);

        // disable the Time Bonus if we exceed chanceForBonus
        if (random > m_chanceForBonus) {
            m_bonusValue = 0;
        }

        // if we are not using a timed, level disable the TimeBonus
        if (GameManager.Instance != null) {
            if (GameManager.Instance.LevelGoal.m_levelCounter == LevelCounter.Moves) {
                m_bonusValue = 0;
            }
        }

        // activate/deactive Ring Glow and Bonus Glow
        SetActive(m_bonusValue != 0);

        // if TimeBonus is active, set the particle material based on the bonusValue
        if (m_bonusValue != 0) {
            SetupMaterial(m_bonusValue - 1, m_bonusGlow);
        }

    }

    // activate or deactive bonusGlow and ringGlow effects
    void SetActive(bool state) {

        if (m_bonusGlow != null) {
            m_bonusGlow.SetActive(state);
        }

        if (m_ringGlow != null) {
            m_ringGlow.SetActive(state);
        }
    }

    // set the material depending on the bonus value
    void SetupMaterial(int _value, GameObject _bonusGlow) {
        
        // avoids Out of Range error
        int clampedValue = Mathf.Clamp(_value, 0, m_bonusMaterials.Length - 1);

        // set the BonusGlow renderer to use the proper material
        if (m_bonusMaterials[clampedValue] != null) {

            if (_bonusGlow != null) {
                ParticleSystemRenderer bonusGlowRenderer = _bonusGlow.GetComponent<ParticleSystemRenderer>();
                bonusGlowRenderer.material = m_bonusMaterials[clampedValue];
            }
        }

    }
}
