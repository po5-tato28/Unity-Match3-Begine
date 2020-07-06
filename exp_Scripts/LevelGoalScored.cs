using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal {

    public override void Start() {
        m_levelCounter = LevelCounter.Moves;
        base.Start();
    }

    public override bool IsWinner()
    {
        if(ScoreManager.Instance != null) {
            return (ScoreManager.Instance.CurrentScore >= m_scoreGoals[0]);
        }
        return false;
    }

    public override bool IsGameOver()
    {
        int maxScore = m_scoreGoals[m_scoreGoals.Length - 1];
        // 별 3개 찍으면 끝!
        if (ScoreManager.Instance.CurrentScore >= maxScore) { 
            return true;
        }

        return (m_movesLeft == 0);
    }
}
