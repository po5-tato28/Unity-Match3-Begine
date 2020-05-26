using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    // Properties
    public int m_ID;

    // Methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
     * 이동()
     * 폭발()
     * 힌트표시()
     */

    private void OnMouseDown()
    {
        Vector3 strength = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOPunchScale(strength, 1f);
    }

}