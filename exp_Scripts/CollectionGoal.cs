using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : MonoBehaviour
{
    public GamePiece m_prefabToCollcet;

    [Range(1, 50)]
    public int m_numberToCollect = 5;

    SpriteRenderer m_spriteRenderer;

    
    void Start() {
        if(m_prefabToCollcet != null) {
            m_spriteRenderer = m_prefabToCollcet.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectPiece(GamePiece _piece) {
        if(_piece != null) {
            SpriteRenderer spriteRenderer = _piece.GetComponent<SpriteRenderer>();
            if (m_spriteRenderer.sprite == m_spriteRenderer.sprite && m_prefabToCollcet.m_matchValue == _piece.m_matchValue)
            {
                m_numberToCollect--;
                m_numberToCollect = Mathf.Clamp(m_numberToCollect, 0, m_numberToCollect);
            }
        }
    }
}
