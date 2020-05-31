// Tile.cs

public class Tile : MonoBehaviour {
	
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

        if (m_IsSelected)  {
            Deselect();
        } else {
            if (m_previousSelected == null) {
                Select();
            } else {
                /*
				SwapSprite(m_previousSelected.m_render);
				m_previousSelected.Deselect();
				*/
                if (GetAllAdjacentTiles().Contains(m_previousSelected.gameObject)) {
                    SwapSprite(m_previousSelected.m_render);
                    m_previousSelected.ClearAllMatches();
                    m_previousSelected.Deselect();
                    ClearAllMatches();
                } else {
                    m_previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapSprite(SpriteRenderer _render) {
        if (m_render.sprite == _render.sprite)  {
            return;
        }

        // Swap Selected Sprites
        Sprite tempSprite = _render.sprite;
        _render.sprite = m_render.sprite;
        m_render.sprite = tempSprite;
        SFXManager.instance.PlaySFX(Clip.Swap);
    }

    private GameObject GetAdjacent(Vector2 _castDir) {

        // Raycast = Raycast 스크립팅을 가진 게임오브젝트의 원점에서 내가 설정한 방향으로 Ray를 날려
        // 내가 설정한 거리 이내에 물체가 있는지 없는지 충돌감지를 해주는 것.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _castDir);

        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }
    private List<GameObject> GetAllAdjacentTiles() {
        List<GameObject> adjacentTiles = new List<GameObject>();

        for (int i = 0; i < m_adjacentDirections.Length; i++) {
            adjacentTiles.Add(GetAdjacent(m_adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 _castDir) {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _castDir);

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == m_render.sprite) {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, _castDir);
        }

        return matchingTiles;
    }

    private void ClearMatch(Vector2[] _paths) {
        List<GameObject> matchingTiles = new List<GameObject>();
        for(int i = 0; i < _paths.Length; i++) {
            matchingTiles.AddRange(FindMatch(_paths[i]));
        }

        if(matchingTiles.Count >= 2) {
            for(int i = 0; i <matchingTiles.Count; i++) {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            m_matchFound = true;
        }
    }

    public void ClearAllMatches() {
        if (m_render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (m_matchFound) {
            m_render.sprite = null;
            m_matchFound = false;

            StopCoroutine(BoardManager.m_instance.FindNullTiles());
            StartCoroutine(BoardManager.m_instance.FindNullTiles());

            SFXManager.instance.PlaySFX(Clip.Clear);
			GUIManager.instance.MoveCounter--;
        }
    }
}