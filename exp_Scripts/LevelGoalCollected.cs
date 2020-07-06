using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalCollected : LevelGoal {

    public CollectionGoal[] m_collectionGoals;
    //public CollectionGoalPanel[] m_uiPanels;


    public void UpdateGoals (GamePiece _pieceTocheck) {

        foreach(CollectionGoal goal in m_collectionGoals){

            if(_pieceTocheck != null) {
                goal.CollectPiece(_pieceTocheck);
            }
        }
        UpdateUI();
        
    }

    public void UpdateUI() {
        /*
        foreach (CollectionGoalPanel panel in m_uiPanels) {
            if (panel != null) {
                panel.UpdatePanel();
            }
        }*/

        if(UIManager.Instance != null) {
            UIManager.Instance.UpdateCollectionGoalLayout();
        }
    }

    bool AreGoalsComplete(CollectionGoal[] _goals) {

            foreach (CollectionGoal g in _goals) {

                if (g == null | _goals == null) {
                    return false;
                }
                if(_goals.Length == 0) {
                    return false;
                }
                if (g.m_numberToCollect != 0) {
                    return false;
                }
            }
        
        return true;
    }

    public override bool IsGameOver() {

        if (AreGoalsComplete(m_collectionGoals) && ScoreManager.Instance != null) {

            int maxScore = m_scoreGoals[m_scoreGoals.Length - 1];
            if(ScoreManager.Instance.CurrentScore >= maxScore) {
                return true;
            }
        }
        if (m_levelCounter == LevelCounter.Timer) {
            return (m_timeLeft <= 0);

        } else {
            return (m_movesLeft <= 0);
        }
    }

    public override bool IsWinner() {

        if (ScoreManager.Instance != null) {
            return (ScoreManager.Instance.CurrentScore >= m_scoreGoals[0] && AreGoalsComplete(m_collectionGoals));
        }
        return false;
    }

}
