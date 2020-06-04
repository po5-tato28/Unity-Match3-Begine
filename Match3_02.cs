// Tile.cs
// permit 허가하다
// generate 발생시키다
// iterate 반복하다

public class Tile : MonoBehaviour {
	
	private Vector2[]       m_adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool            m_matchFound = false; // default -> they found yet.

	void Awake() {
		m_render = GetComponent<SpriteRenderer>();
    }	
	
	private void Select() {
		m_IsSelected = true;
		m_render.color = m_selectedColor;
        m_previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
        m_IsSelected = false;
        m_render.color = Color.white;
        m_previousSelected = null;
	}
	
	void OnMouseDown() {
        if (m_render.sprite == null || BoardManager.m_instance.m_IsShifting) {
            return;
        }

        if (m_IsSelected)  { // Is it a already selected?
            Deselect();
        } else {
            if (m_previousSelected == null) { // Is it the first tile seleced?
                Select();
            } else {
                // ▽ call 'GetAllAdjacentTiles' method, and check game object (does it returned adjacent tiles list?)
				if (GetAllAdjacentTiles().Contains(m_previousSelected.gameObject)) { 
                    SwapSprite(m_previousSelected.m_render); // swap
                    m_previousSelected.ClearAllMatches();
                    m_previousSelected.Deselect(); // if it wasn't the first one that was selected, deselect all tiles.
                    ClearAllMatches();
                } else { // If it doesn't previous, deselect. && instaed select new tile.
                    m_previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapSprite(SpriteRenderer _render) { 
        if (m_render.sprite == _render.sprite)  { // if same each other, return.
            return;
        }

        // Swap Selected Sprites.
        Sprite tempSprite = _render.sprite;
        _render.sprite = m_render.sprite;
        m_render.sprite = tempSprite;
		
        SFXManager.instance.PlaySFX(Clip.Swap); // play sound effect.
    }
	
    private GameObject GetAdjacent(Vector2 _castDir) { // use this method at 'GetAllAdjacentTiles' method.

        // Raycast = Raycast 스크립팅을 가진 게임오브젝트의 원점에서 내가 설정한 방향으로 Ray를 날려
        // 내가 설정한 거리 이내에 물체가 있는지 없는지 충돌감지를 해주는 것.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _castDir); ; // Fire a ray from the towards the _castDir direction.

        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }
	
    private List<GameObject> GetAllAdjacentTiles() {
        List<GameObject> adjacentTiles = new List<GameObject>();

        for (int i = 0; i < m_adjacentDirections.Length; i++) {
			// use 'GetAdjacent' method to generate a list of tiles 
			// surrounding the current tile.
            adjacentTiles.Add(GetAdjacent(m_adjacentDirections[i]));
        }
        return adjacentTiles;
    }

	/// <summary>
    /// FindMatch
    /// </summary>
	/// <param name="_castDir"> accepts a 'Vector2' </param>
    /// <returns> List<GameObject> matchingTiles </returns>
    ///
    private List<GameObject> FindMatch(Vector2 _castDir) {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _castDir);

		// ▽ keep firing raycasts until either raycast hits nothing, and returned object sprite differ each other...
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == m_render.sprite) {
		    matchingTiles.Add(hit.collider.gameObject); // add it at 'matchingTiles' list.
            hit = Physics2D.Raycast(hit.collider.transform.position, _castDir); // re hit.
        }

        return matchingTiles; // return list.
    }
	
	
	/// <summary>
    /// ClearMatch, void
    /// </summary>
	/// <param name="_paths"> accepts a 'Vector2[]'... array </param>
    ///
    private void ClearMatch(Vector2[] _paths) { // these(_paths) are the path of raycast.
        List<GameObject> matchingTiles = new List<GameObject>();
		
		// ▽ until iterate the list of paths, add any matches to the 'matchingTile' list.
        for(int i = 0; i < _paths.Length; i++) {
            matchingTiles.AddRange(FindMatch(_paths[i]));
        }
		
		if(matchingTiles.Count >= 2) { // find match tiles 2 upper (we need match 3 tiles)
            for(int i = 0; i < matchingTiles.Count; i++) { // loop as much as 'matchingTiles.count'
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            m_matchFound = true; // set 'm_matchFound' flag to true
        }
    }

    public void ClearAllMatches() {
        if (m_render.sprite == null)
            return;
		
		// call 'ClearMatch' method for compare vertical and horizontal.
        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right }); // horizontal
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down }); // vertical

        if (m_matchFound) { // m_matchFound == true
            m_render.sprite = null;
            m_matchFound = false;
			
			// ▽ this will stop the 'FindNullTiles' coroutine and start it again.
            StopCoroutine(BoardManager.m_instance.FindNullTiles());
            StartCoroutine(BoardManager.m_instance.FindNullTiles());

            SFXManager.instance.PlaySFX(Clip.Clear);
			GUIManager.instance.MoveCounter--;
        }
    }
}