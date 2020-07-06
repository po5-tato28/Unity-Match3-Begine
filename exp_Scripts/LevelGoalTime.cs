using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTime : LevelGoal
{
    //public Timer m_timer;
    //int m_maxTime;

    public override void Start() {

        m_levelCounter = LevelCounter.Timer;
        base.Start();

    }

    

    public override bool IsWinner() {
        if (ScoreManager.Instance != null) {
            return (ScoreManager.Instance.CurrentScore >= m_scoreGoals[0]);
        }
        return false;
    }

    public override bool IsGameOver() {
        int maxScore = m_scoreGoals[m_scoreGoals.Length - 1];
        // 별 3개 찍으면 끝!
        if (ScoreManager.Instance.CurrentScore >= maxScore) {
            return true;
        }

        return (m_timeLeft <= 0);
    }

}
