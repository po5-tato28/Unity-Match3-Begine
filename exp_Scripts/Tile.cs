using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { 
    Normal,
    Obstacle,
    Breakable,
};

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour { 
    public int m_xIndex;
    public int m_yIndex;

    Board m_board;

    public TileType m_tileType = TileType.Normal;

    SpriteRenderer m_spriteRenderer;

    public int m_breakableValue = 0;
    public Sprite[] m_breakableSprites;
    public Sprite[] m_NormalSprites;

    public Color m_normalColor;


    void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        
    }

    public void Init(int _x, int _y, Board _board) {

        m_xIndex = _x;
        m_yIndex = _y;
        m_board = _board;

        if (m_tileType == TileType.Breakable) {
            if (m_breakableSprites[m_breakableValue] != null) {
                m_spriteRenderer.sprite = m_breakableSprites[m_breakableValue];
            }
        }
    }

    private void OnMouseDown() {
        if (m_board != null) {
            m_board.ClickTile(this);
            // this tile
        }
    }
    private void OnMouseEnter() {
        if (m_board != null) {
            m_board.DragToTile(this); // this tile
        }

    }
    private void OnMouseUp() {
        if (m_board != null) {
            m_board.ReleaseTile();
        }
    }

    public void BreakTile() {
        if(m_tileType != TileType.Breakable) {
            return;
        }
        StartCoroutine(BreakTileRoutine());
    }

    IEnumerator BreakTileRoutine() {

        m_breakableValue = Mathf.Clamp(--m_breakableValue, 0, m_breakableValue);

        yield return new WaitForSeconds(0.25f);

        if (m_breakableSprites[m_breakableValue] != null) {
            m_spriteRenderer.sprite = m_breakableSprites[m_breakableValue];
        }
        if (m_breakableValue == 0) {
            m_tileType = TileType.Normal;
            m_spriteRenderer.sprite = m_NormalSprites[0];
        }
    }

}
