using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject m_clearFXPrefab;
    public GameObject m_breakFXPrefab;
    public GameObject m_doubleBreakFXPrefab;
    public GameObject m_bombFXPrefab;

    public void ClearPieceFXAt(int _x, int _y, int _z = 0) {
        if(m_clearFXPrefab != null) {

            GameObject clearFX = Instantiate(m_clearFXPrefab, new Vector3(_x, _y, _z), Quaternion.identity) as GameObject;

            ParticlePlayer particlePalyer = m_clearFXPrefab.GetComponent<ParticlePlayer>();

            if (particlePalyer != null) {
                particlePalyer.Play();
            }// if2
        }// if1
    }

    public void BreakTileFXAt(int _breakableValue, int _x, int _y, int _z = 0) {
        GameObject breakFX = null;
        ParticlePlayer particlePlayer = null;

        if (_breakableValue > 1) {
            if (m_doubleBreakFXPrefab != null) {
                breakFX = Instantiate(m_doubleBreakFXPrefab, new Vector3(_x, _y, _z), Quaternion.identity) as GameObject;
            }

        } else {
            if (m_breakFXPrefab != null) {
                breakFX = Instantiate(m_breakFXPrefab, new Vector3(_x, _y, _z), Quaternion.identity) as GameObject;
            }
        }//if-else

        if(breakFX != null) {
            particlePlayer = breakFX.GetComponent<ParticlePlayer>();
            if(particlePlayer!=null) {
                particlePlayer.Play();
            }// if2
        }// if1
    }

    public void BombFXAt(int _x, int _y, int _z = 0) {
        if(m_bombFXPrefab != null) {
            GameObject bombFX = Instantiate(m_bombFXPrefab, new Vector3(_x, _y, _z), Quaternion.identity) as GameObject;
            ParticlePlayer particlePlayer = bombFX.GetComponent<ParticlePlayer>();     

            if(particlePlayer != null) {
                particlePlayer.Play();
            }// if2
        }// if1
    }

}
