// BoardManager.cs

public class BoardManager : MonoBehaviour {
	
	public static BoardManager 	m_instance;							// Singleton
	public List<Sprite> 		m_characters = new List<Sprite>(); 	// List of Sprites
	public GameObject 			m_tile; 							// Instantiated prefab 
	public int 					m_xSize, m_ySize;					// dimensions of the board
	
	private GameObject[,] 		m_tileArray; 						// 2D array, used to store the tiles
	
	public bool 				m_IsShifting { get; set; }			// encapsulated bool;
																	// will tell the game when a match is found and
																	// the board is re-filling.
	
	void Start() 
	{
		m_instance = GetComponent<BoardManager>(); // set singleton (with reference of the BoardManager)
		
		Vector2 offset = m_tile.GetComponent<SpriteRenderer>().bounds.size;
		CreateBoard(offset.x, offset.y); // passing in the bounds of the 'm_tile' sprite size.
	}
	
	 private void CreateBoard(float _xOffset, float _yOffset)
    {
        m_tileArray = new GameObject[m_xSize, m_ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[m_ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < m_xSize; x++) {
            for(int y = 0; y < m_ySize; y++) {
                GameObject newTile = Instantiate(m_tile, new Vector3(startX + (_xOffset * x), startY + (_yOffset * y), 0), m_tile.transform.rotation);
                m_tileArray[x, y] = newTile;

                newTile.transform.parent = transform; // 

                List<Sprite> possibleCharacters = new List<Sprite>(); // 1
                possibleCharacters.AddRange(m_characters); // 2

                possibleCharacters.Remove(previousLeft[y]); // 3
                possibleCharacters.Remove(previousBelow);

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
    
    /// <summary>
    /// coroutine ... StartCoroutine
    /// </summary>
    /// <returns> StartCoroutine(ShiftTilesDown(x,y) </returns>
    /// 
    public IEnumerator FindNullTiles() { 
        for(int x = 0; x < m_xSize; x++) {
            for (int y = 0; y < m_ySize; y++) {
                if (m_tileArray[x,y].GetComponent<SpriteRenderer>().sprite == null) {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                }
            }
        }
		
		for(int x = 0; x < m_xSize; x++) {
            for (int y = 0; y < m_ySize; y++) {
                m_tileArray[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    /// <summary>
    /// coroutine ... ShiftTilesDown
    /// </summary>
    /// <param name="_x"> x position </param>
    /// <param name="_yStart"> start y position </param>
    /// <param name="_shiftDelay"> coroutine delay time 0.03f </param>
    /// <returns> WaitForSeconds(_shiftDelay) </returns>
    /// 
    private IEnumerator ShiftTilesDown(int  _x, int _yStart, float _shiftDelay = 0.03f) {

        m_IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for(int y = _yStart; y <m_ySize; y++) {
            SpriteRenderer render = m_tileArray[_x, y].GetComponent<SpriteRenderer>();

            if(render.sprite == null) {
                nullCount++;
            }
            renders.Add(render);
        }

        for(int i = 0; i < nullCount; i++) {
			GUIManager.instance.Score += 50;
            yield return new WaitForSeconds(_shiftDelay);
			
            for(int k = 0; k < renders.Count -1; k++) {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(_x, m_ySize -1);
            }
        }
        m_IsShifting = false;
    }

    private Sprite GetNewSprite(int _x, int _y) {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(m_characters);

        if (_x > 0) {
            possibleCharacters.Remove(m_tileArray[_x - 1, _y].GetComponent<SpriteRenderer>().sprite);
        }
        if(_x < m_xSize - 1) {
            possibleCharacters.Remove(m_tileArray[_x + 1, _y].GetComponent<SpriteRenderer>().sprite);
        }
        if(_y > 0) {
            possibleCharacters.Remove(m_tileArray[_x, _y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
		
	}
}
