using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    int m_currentScore = 0;
    public int CurrentScore {
        get {
            return m_currentScore;
        }
    }

    int m_counterValue = 0;
    int m_increment = 5;
    public float m_countTime = 1f;

    public Text m_scoreText;

    void Start() {
        UpdateScoreText(m_currentScore);
    }

    public void UpdateScoreText(int _scoreValue) {
        if(m_scoreText != null) {
            m_scoreText.text = _scoreValue.ToString();
        }
    }

    public void AddScore(int _value) {
        m_currentScore += _value;
        StartCoroutine(CountScoreRoutine());
    }
    IEnumerator CountScoreRoutine() {

        int iterations = 0;
        while(m_counterValue < m_currentScore && iterations < 10000) {

            m_counterValue += m_increment;
            UpdateScoreText(m_counterValue);
            iterations++;
            yield return null;
        }

        m_counterValue = m_currentScore;
        UpdateScoreText(m_currentScore);
    }
}
