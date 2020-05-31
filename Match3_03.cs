// GUIManager.cs

public class GUIManager : MonoBehaviour {
		
	//
	private int m_moveCounter;

	public int Score {
		get { return m_score; }
		set {
			m_score = value;
			m_scoreTxt.text = m_score.ToString();
		}			
	}
	
	public int MoveCounter {
		get { return m_moveCounter; }
		set {
			m_moveCounter = value;
			m_moveCounterTxt.text = m_moveCounter.ToString();
		}
	}
	

	void Awake() {
		m_instance = GetComponent<GUIManager>();
		
		m_moveCounter = 60;
		m_moveCounterTxt.text = m_moveCounter.ToString();
	}

	// Show the game over panel
	public void GameOver() {
		GameManager.m_instance.gameOver = true;

		m_gameOverPanel.SetActive(true);

		if (m_score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", m_score);
			m_highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			m_highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}

		m_yourScoreTxt.text = m_score.ToString();
	}

}