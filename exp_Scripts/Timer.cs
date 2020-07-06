using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour {

    public Text m_timeLeftText;
    public Image m_clockImage;

    int m_maxTime = 60;
    public bool m_paused = false;

    public int m_flashTimeLimit = 10;
    public AudioClip m_flashBeep;
    public float m_flashInterval = 1f;
    public Color m_flashColor = Color.red;

    IEnumerator m_flashRoutine;


    public void InitTimer(int _maxTime = 60) {
        m_maxTime = _maxTime;
        if(m_clockImage != null) {
            m_clockImage.type = Image.Type.Filled;
            m_clockImage.fillMethod = Image.FillMethod.Radial360;
            m_clockImage.fillOrigin = (int)Image.Origin360.Top;
        }

        if(m_timeLeftText != null) {
            m_timeLeftText.text = _maxTime.ToString();
        }
    }
    
    public void UpdateTimer(int _currentTime) {

        if (m_paused) {
            return;
        }
        if (m_clockImage != null) {
            m_clockImage.fillAmount = (float)_currentTime / (float)m_maxTime;

            if(_currentTime <= m_flashTimeLimit) {

                m_flashRoutine = FlashRoutine(m_clockImage, m_flashColor, m_flashInterval);
                StartCoroutine(m_flashRoutine);

                if (SoundManager.Instance != null && m_flashBeep != null) { 
                    SoundManager.Instance.PlayClipAtPoint(m_flashBeep, Vector3.zero, SoundManager.Instance.m_fxVolume, false);
                }
            }
        }
        if(m_timeLeftText != null) {
            m_timeLeftText.text = _currentTime.ToString();
        }
    }

    IEnumerator FlashRoutine(Image _image, Color _targetColor, float _interval) {

        if (_image != null) {
            Color originalColor = _image.color;
            _image.CrossFadeColor(_targetColor, _interval * 0.3f, true, true);
            yield return YieldInstructionCache.WaitForSeconds(_interval * 0.5f);

            _image.CrossFadeColor(originalColor, _interval * 0.3f, true, true);
            yield return YieldInstructionCache.WaitForSeconds(_interval * 0.5f);
        }
    }

    public void FadeOff() {

        if (m_flashRoutine != null) { 
            StopCoroutine(m_flashRoutine);
        }

        ScreenFader[] screenFaders = GetComponentsInChildren<ScreenFader>();
        foreach(ScreenFader fader in screenFaders) {
            fader.FadeOff();
        }
    }
}
