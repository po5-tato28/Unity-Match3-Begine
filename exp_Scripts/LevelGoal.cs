using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelCounter {
    Timer,
    Moves,
};

public abstract class LevelGoal : Singleton<LevelGoal> {

    public int m_scoreStars = 0;
    public int[] m_scoreGoals = new int[3] { 1000, 2000, 3000 };

    public int m_movesLeft = 30;
    public int m_timeLeft = 60;

    public LevelCounter m_levelCounter = LevelCounter.Moves;

    int m_maxTime;


    public virtual void Start() {
        Init();

        if(m_levelCounter == LevelCounter.Timer) {
            m_maxTime = m_timeLeft;

            if (UIManager.Instance != null && UIManager.Instance.m_timer != null) {
                UIManager.Instance.m_timer.InitTimer(m_timeLeft);
            }
        }
    }

    void Init() {
        m_scoreStars = 0;
        for(int i = 1; i <m_scoreGoals.Length; i++) {
            if(m_scoreGoals[i] < m_scoreGoals[i - 1]) {
                Debug.LogWarning("LEVELGOAL Setup score goals in increasing order!");
            }// if
        }// for
    }

    int UpdateScore(int _score) {
        for(int i = 0; i <m_scoreGoals.Length; i++) {
            if(_score < m_scoreGoals[i]) {
                return i;
            }
        }
        return m_scoreGoals.Length;
    }

    public void UpdateScoreStars(int _score) {
        m_scoreStars = UpdateScore(_score);
    }

    public abstract bool IsWinner();
    public abstract bool IsGameOver();

    //LevelGoalTime
    public void StartCountdown(){
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        while (m_timeLeft > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            m_timeLeft--;

            if (UIManager.Instance != null && UIManager.Instance.m_timer != null)
            {
                UIManager.Instance.m_timer.UpdateTimer(m_timeLeft);
            }
        }
    }

    public void AddTime(int _timeValue)
    {
        m_timeLeft += _timeValue;
        m_timeLeft = Mathf.Clamp(m_timeLeft, 0, m_maxTime);

        if (UIManager.Instance != null && UIManager.Instance.m_timer != null)
        {
            UIManager.Instance.m_timer.UpdateTimer(m_timeLeft);
        }
    }
}
