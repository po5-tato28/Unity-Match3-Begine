using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject m_collectionGoalLayout;
    public int m_collectionGoalBaseWidth = 125;
    CollectionGoalPanel[] m_collectionGoalPanels;

    public ScreenFader m_screenFader;

    public Text m_stageNameText;
    public Text m_movesLeftText;

    public ScoreMeter m_scoreMeter;
    public MessageWindow m_messageWindow;

    public GameObject m_movesCounter;

    public GameObject m_menuSet; // sub menu

    public Timer m_timer;

    
    public override void Awake() {
        base.Awake();
        if(m_messageWindow!= null) {
            m_messageWindow.gameObject.SetActive(true);
            m_messageWindow.SetupStarResult();
        }
        if (m_screenFader != null) {
            m_screenFader.gameObject.SetActive(true);
        }
    }

    void Update() {
        
        // sub menu
        if (Input.GetButtonDown("Cancel")) {
            if (m_menuSet.activeSelf) {
                m_menuSet.SetActive(false);
            } else {
                m_menuSet.SetActive(true);
            }
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] _collectionGoals, GameObject _goalLayout, int _spacingWidth) {

        if (m_collectionGoalLayout != null && _collectionGoals != null && _collectionGoals.Length != 0) {

            RectTransform rectXform = _goalLayout.GetComponent<RectTransform>();
            rectXform.sizeDelta = new Vector2(_collectionGoals.Length * _spacingWidth, rectXform.sizeDelta.y);

            CollectionGoalPanel[] panels = _goalLayout.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < panels.Length; i++) {
                if (i < _collectionGoals.Length && _collectionGoals[i] != null) {

                    panels[i].gameObject.SetActive(true);
                    panels[i].m_collectionGoal = _collectionGoals[i];
                    panels[i].SetupPanel();

                } else {
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] _collectionGoals) {
        SetupCollectionGoalLayout(_collectionGoals, m_collectionGoalLayout, m_collectionGoalBaseWidth);
    }


    public void UpdateCollectionGoalLayout(GameObject _goalLayout) {

        if(_goalLayout != null) {
            CollectionGoalPanel[] panels = _goalLayout.GetComponentsInChildren<CollectionGoalPanel>();

            if (panels != null && panels.Length != 0)
            {
                foreach (CollectionGoalPanel panel in panels)
                {
                    if(panel != null && panel.isActiveAndEnabled) {
                        panel.UpdatePanel();
                    }// if3
                }// foreach
            }//if2
        }// if1
    }


    public void UpdateCollectionGoalLayout() {
        UpdateCollectionGoalLayout(m_collectionGoalLayout);
    }

    public void EnableTimer(bool _state) {

        if (m_timer != null) {
            m_timer.gameObject.SetActive(_state);
        }
    }

    public void EnableMovesCounter(bool _state) {

        if (m_movesCounter != null) {
            m_movesCounter.SetActive(_state);
        }
    }

    public void EnableCollectionGoalLayout(bool _state) {
        
        if(m_collectionGoalLayout != null) {
            m_collectionGoalLayout.SetActive(_state);
        }
    }

}
