using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    // reference to slider component
    public Slider m_slider;

    // array of ScoreStar components (defaults to three stars)
    public ScoreStar[] m_scoreStars = new ScoreStar[3];

    // reference to LevelGoal component
    LevelGoal m_levelGoal;

    // reference to maximum score (largest scoring goal)
    int m_maxScore;

    void Awake() {
        // populate the Slider component
        m_slider = GetComponent<Slider>();
    }

    // position the ScoreStars automatically
    public void SetupStars(LevelGoal _levelGoal) {

        // if levelGoal is invalid, return immediately
        if (_levelGoal == null) {
            Debug.LogWarning("SCOREMETER Invalid level goal!");
            return;
        }

        // cache the LevelGoal component for later
        m_levelGoal = _levelGoal;

        // set the maximum score goal
        m_maxScore = m_levelGoal.m_scoreGoals[m_levelGoal.m_scoreGoals.Length - 1];

        // get the slider's RectTransform width
        float sliderWidth = m_slider.GetComponent<RectTransform>().rect.width;

        // avoid divide by zero error
        if (m_maxScore > 0) {
            // loop through our scoring goals
            for (int i = 0; i < _levelGoal.m_scoreGoals.Length; i++) {
                // if the corresponding ScoreStar exists...
                if (m_scoreStars[i] != null) {

                    // set the x value based on the ratio of the scoring goal over the maximum score
                    float newX = (sliderWidth * _levelGoal.m_scoreGoals[i] / m_maxScore) - (sliderWidth * 0.5f);

                    // move the ScoreStar's RectTransform
                    RectTransform starRectXform = m_scoreStars[i].GetComponent<RectTransform>();

                    if (starRectXform != null) {
                        starRectXform.anchoredPosition = new Vector2(newX, starRectXform.anchoredPosition.y);

                    }// if3
                }// if2
            }// for
        }// if1

    }

    // Update the ScoreMeter 
    public void UpdateScoreMeter(int _score, int _starCount) {
        if (m_levelGoal != null)
        {
            // adjust the slider fill area (cast as floats, otherwise will become zero)
            m_slider.value = (float)_score / (float)m_maxScore;
        }

        // activate each star based on current star count
        for (int i = 0; i < _starCount; i++)
        {
            if (m_scoreStars[i] != null)
            {
                m_scoreStars[i].Activate();
            }
        }
    }
}
