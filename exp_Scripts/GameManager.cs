using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager> {

    //public int m_movesLeft = 30;
    //public int m_scoreGoal = 10000;

    Board m_board;

    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    public bool IsGameOver {
        get {
            return m_isGameOver;
        }
        set {
            m_isGameOver = value;
        }
    }

    bool m_isWinner = false;
    bool m_isReadyToReload = false;

    LevelGoal m_levelGoal;
    //LevelGoalTime m_levelGoalTime;

    LevelGoalCollected m_levelGoalCollected;


    //method
    
    public LevelGoal LevelGoal { get { return m_levelGoal; } }

    public override void Awake() {
        base.Awake();

        m_levelGoal = GetComponent<LevelGoal>();
        m_levelGoalCollected = GetComponent<LevelGoalCollected>();

        // cache a reference to the Board
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    void Start() {

        if(UIManager.Instance != null) {
            if (UIManager.Instance.m_scoreMeter != null) {
                UIManager.Instance.m_scoreMeter.SetupStars(m_levelGoal);
            }

            if (UIManager.Instance.m_stageNameText != null) {
                Scene scene = SceneManager.GetActiveScene();
                UIManager.Instance.m_stageNameText.text = scene.name;
            }
            if (m_levelGoalCollected != null) {
                UIManager.Instance.EnableCollectionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(m_levelGoalCollected.m_collectionGoals);
            } else {
                UIManager.Instance.EnableCollectionGoalLayout(false);
            }

            bool useTimer = (m_levelGoal.m_levelCounter == LevelCounter.Timer);

            UIManager.Instance.EnableTimer(useTimer);
            UIManager.Instance.EnableMovesCounter(!useTimer);
        }       



        m_levelGoal.m_movesLeft++;
        UpdateMoves();
        StartCoroutine("ExecuteGameLoop");
    }

    public void UpdateMoves() {

        if (m_levelGoal.m_levelCounter == LevelCounter.Moves) {
            m_levelGoal.m_movesLeft--;

            if (UIManager.Instance != null && UIManager.Instance.m_movesLeftText != null) {
                UIManager.Instance.m_movesLeftText.text = m_levelGoal.m_movesLeft.ToString() + " MOVES";
            }
        } 
    }

    IEnumerator ExecuteGameLoop() {

        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");

        // wait for board to refill
        yield return StartCoroutine("WaitForBoardRoutine", 0.5f);

        yield return StartCoroutine("EndGameRoutine");
    }

    public void BeginGame() {
        m_isReadyToBegin = true;
    }

    IEnumerator StartGameRoutine() {

        if(UIManager.Instance != null) {
            if (UIManager.Instance.m_messageWindow != null) {
                UIManager.Instance.m_messageWindow.GetComponent<RectXformMover>().MoveOn();

                int maxGoal = m_levelGoal.m_scoreGoals.Length - 1;
                UIManager.Instance.m_messageWindow.ShowScoredMessage(m_levelGoal.m_scoreGoals[maxGoal]);

                if(m_levelGoal.m_levelCounter == LevelCounter.Timer) {
                    UIManager.Instance.m_messageWindow.ShowTimedGoal(m_levelGoal.m_timeLeft);
                } else {
                    UIManager.Instance.m_messageWindow.ShowMovesGoal(m_levelGoal.m_movesLeft);
                }
                
                if(m_levelGoalCollected != null) {
                    UIManager.Instance.m_messageWindow.ShowCollectionGoal(true);

                    GameObject goalLayout = UIManager.Instance.m_messageWindow.m_collectionGoalLayout;
                    if(goalLayout!= null) {
                        UIManager.Instance.SetupCollectionGoalLayout(m_levelGoalCollected.m_collectionGoals, goalLayout, 19);
                    }

                } // if3
            }// if2
        }//if1
        while (!m_isReadyToBegin) {
            yield return null;
        }
        // fade off the ScreenFader
        if (UIManager.Instance != null && UIManager.Instance.m_screenFader != null) {
            UIManager.Instance.m_screenFader.FadeOff();
        }

        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        if (m_board != null)  {
            m_board.SetupBoard();
        }
    }

    IEnumerator PlayGameRoutine() {
        if (m_levelGoal.m_levelCounter == LevelCounter.Timer) {
            m_levelGoal.StartCountdown();
        }

        while (!m_isGameOver) {
            m_isGameOver = m_levelGoal.IsGameOver();
            m_isWinner = m_levelGoal.IsWinner();

            yield return null;
        }
    }

    IEnumerator WaitForBoardRoutine(float _delay = 0f) {

        if (m_levelGoal.m_levelCounter == LevelCounter.Timer && UIManager.Instance != null
            && UIManager.Instance.m_timer != null) {

            UIManager.Instance.m_timer.FadeOff();
            UIManager.Instance.m_timer.m_paused = true;
            
        }
        if (m_board != null) {

            yield return YieldInstructionCache.WaitForSeconds(m_board.swapTime);

            while (m_board.isRefilling) {
                yield return null;
            }
        }
        yield return YieldInstructionCache.WaitForSeconds(_delay);
    }

    IEnumerator EndGameRoutine() {

        m_isReadyToReload = false;

        if (m_isWinner)
        {
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
        }

        yield return YieldInstructionCache.WaitForSeconds(1f);
        // fade the screen 
        if (UIManager.Instance != null && UIManager.Instance.m_screenFader != null) {
            UIManager.Instance.m_screenFader.FadeOn();
        }
        while (!m_isReadyToReload) {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ShowLoseScreen() {

        if (UIManager.Instance != null && UIManager.Instance.m_messageWindow != null) {
            UIManager.Instance.m_messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.m_messageWindow.ShowLoseMessage();
            UIManager.Instance.m_messageWindow.ShowCollectionGoal(false);
            UIManager.Instance.m_messageWindow.ActiveStarResult();

            string caption = "";
            if(m_levelGoal.m_levelCounter == LevelCounter.Timer) {
                caption = "OUT OF TIME!";
            } else {
                caption = "OUT OF MOVES!";
            }
            UIManager.Instance.m_messageWindow.ShowGoalCaption(caption, 0, 0);

            if (UIManager.Instance.m_messageWindow.m_goalFailedIcon != null) {
                UIManager.Instance.m_messageWindow.ShowGoalImage(UIManager.Instance.m_messageWindow.m_goalFailedIcon);
            }

        }
        if (SoundManager.Instance != null) {
            SoundManager.Instance.PlayLoseSound();
        }
    }

    void ShowWinScreen() {

        if (UIManager.Instance != null && UIManager.Instance.m_messageWindow != null) {

            UIManager.Instance.m_messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.m_messageWindow.ShowWinMessage();
            UIManager.Instance.m_messageWindow.ShowCollectionGoal(false);

            if (ScoreManager.Instance != null) {
                string scoreStr = "YOU GET "+ LevelGoal.Instance.m_scoreStars + " HEARTS!";
                UIManager.Instance.m_messageWindow.ShowGoalCaption(scoreStr, 0, 0);
                UIManager.Instance.m_messageWindow.ActiveStarResult(m_levelGoal.m_scoreStars);
            }
            if (UIManager.Instance.m_messageWindow.m_goalCompleteIcon != null) {
                UIManager.Instance.m_messageWindow.ShowGoalImage(UIManager.Instance.m_messageWindow.m_goalCompleteIcon);
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWinSound();
        }
    }

    public void ReloadScene() {
        m_isReadyToReload = true;        
    }

    public void ScorePoints(GamePiece _piece)
    {
        if (_piece != null) {
            if (ScoreManager.Instance != null) {
                //score points
                ScoreManager.Instance.AddScore(_piece.m_scoreValue);

                // update the scoreStars in the Level Goal component
                m_levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);

                if (UIManager.Instance != null && UIManager.Instance.m_scoreMeter != null) {
                    UIManager.Instance.m_scoreMeter.UpdateScoreMeter(ScoreManager.Instance.CurrentScore, m_levelGoal.m_scoreStars);
                }
            }

            if (SoundManager.Instance != null) {
                SoundManager.Instance.PlayClipAtPoint(_piece.m_clearSound, Vector3.zero, SoundManager.Instance.m_fxVolume);
            }
        }
    }

    public void AddTime(int _timeValue) {

        if (m_levelGoal.m_levelCounter == LevelCounter.Timer) {
            m_levelGoal.AddTime(_timeValue);
        }
    }

    public void UpdateCollectionGoals(GamePiece _pieceToCheck) {

        if (m_levelGoalCollected != null) {
            m_levelGoalCollected.UpdateGoals(_pieceToCheck);
        }
    }

    // quit
    public void GameExit() {
        Application.Quit();
    }

    // load scene 임시
    public void GameRobi() {
        // LoadScene("Robi");
    }
    public void GameReLoad() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // save
    public void GameSave() {
        PlayerPrefs.Save();
        UIManager.Instance.m_menuSet.SetActive(false);
    }
    public void GameLoad() {

    }
}

