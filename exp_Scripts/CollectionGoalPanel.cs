using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGoalPanel : MonoBehaviour {

    public CollectionGoal m_collectionGoal;
    public Text m_numberLeftText;
    public Image m_prefabImage;

    void Start() {
        SetupPanel();
    }

    public void SetupPanel() {
        if(m_collectionGoal != null && m_numberLeftText != null && m_prefabImage != null) {

            SpriteRenderer prefabSprite = m_collectionGoal.m_prefabToCollcet.GetComponent<SpriteRenderer>();
            if(prefabSprite != null) { 
                m_prefabImage.sprite = prefabSprite.sprite;
                m_prefabImage.color = prefabSprite.color;
            }
            m_numberLeftText.text = m_collectionGoal.m_numberToCollect.ToString();

        }
    }

    public void UpdatePanel() {

        if(m_collectionGoal != null & m_numberLeftText != null) {
            m_numberLeftText.text = m_collectionGoal.m_numberToCollect.ToString();
        }
    }

}
