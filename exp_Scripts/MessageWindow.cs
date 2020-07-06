using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MessageWindow : MonoBehaviour
{
    public Image m_messageImage;
    public Text m_messageText;
    public Text m_buttonText;

    public Sprite m_loseIcon;
    public Sprite m_winIcon;
    public Sprite m_goalIcon;

    public Sprite m_goalCompleteIcon;
    public Sprite m_goalFailedIcon;

    public Sprite m_collectIcon;
    public Sprite m_timerIcon;
    public Sprite m_movesIcon;

    public Image m_goalImage;
    public Text m_goalText;

    public GameObject m_collectionGoalLayout;

    public ScoreStar[] m_scoreStarsResult = new ScoreStar[3];

    //method

    public void ShowMessage(Sprite _sprite = null, string _message = "", string _buttonMsg = "START") {

        if(m_messageImage != null) {
            m_messageImage.sprite = _sprite;
        }
        if(m_messageText != null) {
            m_messageText.text = _message;
        }
        if(m_buttonText != null) {
            m_buttonText.text = _buttonMsg;
        }
    }

    public void ShowScoredMessage(int _scoreGoal) {
        string message = "COLLECT";
        ShowMessage(m_goalIcon, message, "START");
    }

    public void ShowWinMessage() {
        ShowMessage(m_winIcon, "MISSION COMPLETE", "OK");
    }
    public void ShowLoseMessage()
    {
        ShowMessage(m_loseIcon, "MISSION FAILED", "OK");
    }

    public void ShowGoal(string _caption = "", Sprite _icon = null) {

        if (_caption != "") {
            ShowGoalCaption(_caption);
        }
        if(_icon != null) {
            ShowGoalImage(_icon);
        }
    }

    public void ShowGoalCaption(string _caption = "", int _offsetX = 0, int _offsetY = 0) {
        if (m_goalText != null) {
            m_goalText.text = _caption;
            RectTransform rectXform = m_goalText.GetComponent<RectTransform>();
            rectXform.anchoredPosition += new Vector2(_offsetX, _offsetY);
        }
    }

    public void ShowGoalImage(Sprite _icon = null) {
        if(m_goalImage != null) {
            m_goalImage.gameObject.SetActive(true);
            m_goalImage.sprite = _icon;
        }
        if(_icon == null) {
            m_goalImage.gameObject.SetActive(false);
        }
    }

    public void ShowTimedGoal(int _time) {
        string caption = _time.ToString() + " SECONDS";
        ShowGoal(caption, m_timerIcon);
    }
    public void ShowMovesGoal(int _moves) {
        string caption = _moves.ToString() + " MOVES";
        ShowGoal(caption, m_movesIcon);
    }
    public void ShowCollectionGoal(bool _state = true) {

        if(m_collectionGoalLayout != null) {
            m_collectionGoalLayout.SetActive(_state);
        }
        if (_state) { 
            ShowGoal("", m_collectIcon);
        }
    }

    public void SetupStarResult() {
        if(m_scoreStarsResult != null) { 
            for (int i = 0; i < LevelGoal.Instance.m_scoreGoals.Length; i++) {
                m_scoreStarsResult[i].gameObject.SetActive(false);
            }
        }
    }
    public void ActiveStarResult(int _starCount = 0) {
        if (m_scoreStarsResult != null) {
            for (int i = 0; i < LevelGoal.Instance.m_scoreGoals.Length; i++) {
                m_scoreStarsResult[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < _starCount; i++) {
                m_scoreStarsResult[i].Activate();
            }
        }
    }
}
