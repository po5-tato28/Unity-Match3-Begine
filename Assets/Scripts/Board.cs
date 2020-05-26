using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // 이차원배열
    //private Tile[,] m_Array = new Tile[6,6];

    // 딕셔너리 < Key, Value >
    private Dictionary<string, Tile> m_TilesDictionary = new Dictionary<string, Tile>();
    private GameObject m_TilePrefab;

    public int m_Width  = 16;
    public int m_Height = 16; 

    // Start is called before the first frame update
    void Start()
    {
        m_TilePrefab = Resources.Load("Prefabs/ball1") as GameObject;
        CreateTiles();
    }

    /// <summary>
    /// 프리팹을 이용하여 새로운 타일들을 생성한다.
    /// </summary>
    /// 
    private void CreateTiles()
    {
        for (int y = 0; y < m_Height; y++)
        {
            for (int x = 0; x < m_Width; x++)
            {
                // key 값 예시 : x, y = 10, 2
                string key = x.ToString() + ", " + y.ToString();

                Tile tile = Instantiate<Tile>(m_TilePrefab.transform.GetComponent<Tile>());
                
                tile.transform.SetParent(this.transform); //부모
                tile.transform.position = new Vector3(x, y, 0f);

                m_TilesDictionary.Add(key, tile);
            }
        }
        
    }

    // 데이터 입력 => 처리 => 출력 (output)

    /// <summary>
    /// Tile을 반환한다.
    /// </summary>
    /// <param name="_x">좌표</param>
    /// <param name="_y">좌표</param>
    /// <returns>Tile</returns>

    public Tile GetTile(int _x, int _y)
    {
        string key = _x.ToString() + "," + _y.ToString();

        return m_TilesDictionary[key];
    }

    /// <summary>
    /// Tile을 반환한다.
    /// </summary>
    /// <param name="_xy">좌표</param>
    /// <returns>Tile</returns>
    
    public Tile GetTile(string _xy)
    {
        return m_TilesDictionary[_xy];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
