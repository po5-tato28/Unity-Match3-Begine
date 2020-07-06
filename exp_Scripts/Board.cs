using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoardDeadlock))]
[RequireComponent(typeof(BoardShuffler))]
public class Board : MonoBehaviour
{

    // dimensions of board
    public int width;
    public int height;

    // margin outside Board for calculating camera field of view
    public int borderSize;

    // Prefab representing a single tile
    public GameObject tileNormalPrefab;

    // Prefab representing an empty, unoccupied tile
    public GameObject tileObstaclePrefab;

    // array of dot Prefabs
    public GameObject[] gamePiecePrefabs;

    // Prefab array for adjacent bombs
    public GameObject[] adjacentBombPrefabs;

    // Prefab array for column clearing bombs
    public GameObject[] columnBombPrefabs;

    // Prefab array for row clearing bombs
    public GameObject[] rowBombPrefabs;

    // Prefab bomb FX for clearing a single color from the Board
    public GameObject colorBombPrefab;


    // reference to a Bomb created on the clicked Tile (first Tile clicked by mouse or finger)
    GameObject m_clickedTileBomb;

    // reference to a Bomb created on the target Tile (Tile dragged into by mouse or finger)
    GameObject m_targetTileBomb;

    // the time required to swap GamePieces between the Target and Clicked Tile
    public float swapTime = 0.5f;

    // array of all the Board's Tile pieces
    Tile[,] m_allTiles;

    // array of all of the Board's GamePieces
    GamePiece[,] m_allGamePieces;

    // Tile first clicked by mouse or finger
    Tile m_clickedTile;

    // adjacent Tile dragged into by mouse or finger
    Tile m_targetTile;

    // whether user input is currently allowed
    bool m_playerInputEnabled = true;

    // manually positioned Tiles, placed before Board is filled
    public StartingObject[] startingTiles;

    // manually positioned GamePieces, placed before the Board is filled
    public StartingObject[] startingGamePieces;

    // manager class for particle effects
    ParticleManager m_particleManager;

    // Y Offset used to make the pieces "fall" into place to fill the Board
    public int fillYOffset = 10;

    // time used to fill the Board
    public float fillMoveTime = 0.5f;

    // the current score multiplier, depending on how many chain reactions we have caused
    int m_scoreMultiplier = 0;

    public bool isRefilling = false;

    BoardDeadlock m_boardDeadlock;

    BoardShuffler m_boardShuffler;


    // this is a generic GameObject that can be positioned at coordinate (x,y,z) when the game begins
    [System.Serializable]
    public class StartingObject
    {
        public GameObject prefab;
        public int x;
        public int y;
        public int z;
    }

    // invoked when we start the level
    void Start()
    {
        // initialize array of Tiles
        m_allTiles = new Tile[width, height];

        // initial array of GamePieces
        m_allGamePieces = new GamePiece[width, height];

        // find the ParticleManager by Tag
        m_particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();

        m_boardDeadlock = GetComponent<BoardDeadlock>();

        m_boardShuffler = GetComponent<BoardShuffler>();
    }

    // This function sets up the Board.
    public void SetupBoard()
    {
        // sets up any manually placed Tiles
        SetupTiles();

        // sets up any manually placed GamePieces
        SetupGamePieces();

        // place our Camera to frame the Board with a certain border
        //SetupCamera();

        // fill the empty Tiles of the Board with GamePieces
        FillBoard(fillYOffset, fillMoveTime);
    }

    // Creates a GameObject prefab at certain (x,y,z) coordinate
    void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && IsWithinBounds(x, y))
        {
            // create a Tile at position (x,y,z) with no rotations; rename the Tile and parent it 

            // to the Board, then initialize the Tile into the m_allTiles array

            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            tile.name = "Tile (" + x + "," + y + ")";
            m_allTiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            m_allTiles[x, y].Init(x, y, this);
        }
    }

    // Creates a GamePiece prefab at a certain (x,y,z) coordinate
    void MakeGamePiece(GameObject prefab, int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && IsWithinBounds(x, y))
        {
            prefab.GetComponent<GamePiece>().Init(this);
            PlaceGamePiece(prefab.GetComponent<GamePiece>(), x, y);

            // allows the GamePiece to be placed higher than the Board, so it can be moved into place


            if (falseYOffset != 0)
            {
                prefab.transform.position = new Vector3(x, y + falseYOffset, 0);
                prefab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

            // parent the GamePiece to the Board
            prefab.transform.parent = transform;
        }
    }

    // creat a Bomb prefab at location (x,y)
    GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && IsWithinBounds(x, y))
        {
            // create a Bomb and initialize it; parent it to the Board

            GameObject bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }

    // setup the manually placed Tiles
    void SetupTiles()
    {
        foreach (StartingObject sTile in startingTiles)
        {
            if (sTile != null)
            {
                MakeTile(sTile.prefab, sTile.x, sTile.y, sTile.z);
            }

        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allTiles[i, j] == null)
                {
                    MakeTile(tileNormalPrefab, i, j);
                }
            }
        }
    }

    // setup the manually placed GamePieces
    void SetupGamePieces()
    {
        foreach (StartingObject sPiece in startingGamePieces)
        {
            if (sPiece != null)
            {
                GameObject piece = Instantiate(sPiece.prefab, new Vector3(sPiece.x, sPiece.y, 0), Quaternion.identity) as GameObject;
                MakeGamePiece(piece, sPiece.x, sPiece.y, fillYOffset, fillMoveTime);
            }

        }
    }

    // set the Camera position and parameters to center the Board onscreen with a border
    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;

        float verticalSize = (float)height / 2f + (float)borderSize;

        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }

    // return a random object from an array of GameObjects
    GameObject GetRandomObject(GameObject[] objectArray)
    {
        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning("ERROR:  BOARD.GetRandomObject at index " + randomIdx + "does not contain a valid GameObject!");
        }
        return objectArray[randomIdx];
    }

    // return a random GamePiece
    GameObject GetRandomGamePiece()
    {
        return GetRandomObject(gamePiecePrefabs);
    }

    // place a GamePiece onto the Board at position (x,y)
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD:  Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    // returns true if within the boundaries of the Board, otherwise returns false
    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    // creates a random GamePiece at position (x,y)
    GamePiece FillRandomGamePieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }


    // fill the Board using a known list of GamePieces instead of Instantiating new prefabs
    void FillBoardFromList(List<GamePiece> gamePieces)
    {
        // create a first in-first out Queue to store the GamePieces in a pre-set order
        Queue<GamePiece> unusedPieces = new Queue<GamePiece>(gamePieces);

        // iterations to prevent infinite loop
        int maxIterations = 100;
        int iterations = 0;

        // loop through each position on the Board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // only fill in a GamePiece if 
                if (m_allGamePieces[i, j] == null && m_allTiles[i, j].m_tileType != TileType.Obstacle)
                {
                    // grab a new GamePiece from the Queue
                    m_allGamePieces[i, j] = unusedPieces.Dequeue();

                    // reset iteration count
                    iterations = 0;

                    // while a match forms when filling in a GamePiece...
                    while (HasMatchOnFill(i, j))
                    {
                        // put the GamePiece back into the Queue at the end of the line
                        unusedPieces.Enqueue(m_allGamePieces[i, j]);

                        // grab a new GamePiece from the Queue
                        m_allGamePieces[i, j] = unusedPieces.Dequeue();

                        // increment iterations each time we try to place a piece
                        iterations++;

                        // if our iterations exceeds limit, break out of the loop and move to next position
                        if (iterations >= maxIterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    // fills the empty spaces in the Board with an optional Y offset to make the pieces drop into place
    void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {

        int maxInterations = 100;
        int iterations = 0;

        // loop through all spaces of the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                // if the space is unoccupied and does not contain an Obstacle tile 			
                if (m_allGamePieces[i, j] == null && m_allTiles[i, j].m_tileType != TileType.Obstacle)
                {
                    //GamePiece piece = null;

                    FillRandomGamePieceAt(i, j, falseYOffset, moveTime);
                    iterations = 0;

                    // if we form a match while filling in the new piece...
                    while (HasMatchOnFill(i, j))
                    {
                        // remove the piece and try again
                        ClearPieceAt(i, j);
                        FillRandomGamePieceAt(i, j, falseYOffset, moveTime);

                        // check to prevent infinite loop
                        iterations++;

                        if (iterations >= maxInterations)
                        {
                            break;
                        }
                    }
                    
                }
            }
        }
    }

    // 내부에 연쇄할 폭탄이 있는지?
    private bool HasBombInside(List<GamePiece> gamePieces) {

        foreach (var piece in gamePieces) {
            if (piece != null) {
                Bomb bomb = piece.GetComponent<Bomb>();
                if (bomb != null && bomb.m_bombType != BombType.None) {
                    return true;
                }
            }
        }

        return false;
    }

    // check if we form a match down or to the left when filling the Board
    // note: this does not take into account StartingGamePieces
    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        // find matches to the left
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);

        // find matches downward
        List<GamePiece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        // return whether matches were found
        return (leftMatches.Count > 0 || downwardMatches.Count > 0);

    }

    // set our clicked tile
    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
        }
    }

    // set our target tile
    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    // Swap Tiles if we release the touch/mouse and have valid clicked and target Tiles
    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    // swap two tiles
    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    // coroutine for swapping two Tiles
    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        // if the player input is enabled...
        if (m_playerInputEnabled && !GameManager.Instance.IsGameOver)
        {
            // set the corresponding GamePieces to the clicked Tile and target Tile
            GamePiece clickedPiece = m_allGamePieces[clickedTile.m_xIndex, clickedTile.m_yIndex];
            GamePiece targetPiece = m_allGamePieces[targetTile.m_xIndex, targetTile.m_yIndex];

            if (targetPiece != null && clickedPiece != null)
            {
                // move the clicked GamePiece to the target GamePiece and vice versa
                clickedPiece.Move(targetTile.m_xIndex, targetTile.m_yIndex, swapTime);
                targetPiece.Move(clickedTile.m_xIndex, clickedTile.m_yIndex, swapTime);

                // wait for the swap time
                yield return new WaitForSeconds(swapTime);

                // find all matches for each GamePiece after the swap
                List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.m_xIndex, clickedTile.m_yIndex);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.m_xIndex, targetTile.m_yIndex);

                #region color bombs
                // create a new List to hold potential color matches
                List<GamePiece> colorMatches = new List<GamePiece>();

                // if the clicked GamePiece is a Color Bomb, set the color matches to the first color
                if (IsColorBomb(clickedPiece) && !IsColorBomb(targetPiece))
                {
                    clickedPiece.m_matchValue = targetPiece.m_matchValue;
                    colorMatches = FindAllMatchValue(clickedPiece.m_matchValue);
                }
                //... if the target GamePiece is a Color Bomb, set the color matches to the second color
                else if (!IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    targetPiece.m_matchValue = clickedPiece.m_matchValue;
                    colorMatches = FindAllMatchValue(targetPiece.m_matchValue);
                }
                //... otherwise, if they are both Color bombs, choose the whole Board!
                else if (IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    foreach (GamePiece piece in m_allGamePieces)
                    {
                        if (!colorMatches.Contains(piece))
                        {
                            colorMatches.Add(piece);
                        }
                    }
                }
                #endregion

                // if we don't make any matches, then swap the pieces back
                if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0 && colorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.m_xIndex, clickedTile.m_yIndex, swapTime);
                    targetPiece.Move(targetTile.m_xIndex, targetTile.m_yIndex, swapTime);
                }
                else
                {

                    // wait for our swap time
                    yield return new WaitForSeconds(swapTime);

                    #region drop bombs
                    // record the general vector of our swipe
                    Vector2 swipeDirection = new Vector2(targetTile.m_xIndex - clickedTile.m_xIndex, targetTile.m_yIndex - clickedTile.m_yIndex);

                    // convert the clicked GamePiece or target GamePiece to a bomb depending on matches and swipe direction
                    m_clickedTileBomb = DropBomb(clickedTile.m_xIndex, clickedTile.m_yIndex, swipeDirection, clickedPieceMatches);
                    m_targetTileBomb = DropBomb(targetTile.m_xIndex, targetTile.m_yIndex, swipeDirection, targetPieceMatches);

                    // if the clicked GamePiece is a non-color Bomb, then change its color to the correct target color
                    if (m_clickedTileBomb != null && targetPiece != null)
                    {
                        GamePiece clickedBombPiece = m_clickedTileBomb.GetComponent<GamePiece>();
                        if (!IsColorBomb(clickedBombPiece))
                        {
                            clickedBombPiece.ChangeColor(targetPiece);
                        }
                    }

                    // if the target GamePiece is a non-color Bomb, then change its color to the correct clicked color
                    if (m_targetTileBomb != null && clickedPiece != null)
                    {
                        GamePiece targetBombPiece = m_targetTileBomb.GetComponent<GamePiece>();

                        if (!IsColorBomb(targetBombPiece))
                        {
                            targetBombPiece.ChangeColor(clickedPiece);
                        }
                    }
                    #endregion

                    // clear matches and refill the Board
                    List<GamePiece> piecesToClear = clickedPieceMatches.Union(targetPieceMatches).ToList().Union(colorMatches).ToList();

                    yield return StartCoroutine(ClearAndRefillBoardRoutine(piecesToClear));


                    // otherwise, we decrement our moves left
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.UpdateMoves();
                    }

                }
            }
        }

    }

    // return true if one Tile is adjacent to another, otherwise returns false
    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.m_xIndex - end.m_xIndex) == 1 && start.m_yIndex == end.m_yIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.m_yIndex - end.m_yIndex) == 1 && start.m_xIndex == end.m_xIndex)
        {
            return true;
        }

        return false;
    }

    // general method to find matches, defaulting to a minimum of three-in-a-row, passing in an (x,y) position and direction

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        // keep a running list of GamePieces
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        // get a starting piece at an (x,y) position in the array of GamePieces
        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        // use the search direction to increment to the next space to look...
        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            // ... find the adjacent GamePiece and check its MatchValue...
            GamePiece nextPiece = m_allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }

            // ... if it matches then add it our running list of GamePieces
            else
            {
                if (nextPiece.m_matchValue == startPiece.m_matchValue && !matches.Contains(nextPiece) && nextPiece.m_matchValue != MatchValue.None)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        // if our list is greater than our minimum (usually 3), then return the list...
        if (matches.Count >= minLength)
        {
            return matches;
        }

        //...otherwise return nothing
        return null;

    }

    // find all vertical matches given a position (x,y) in the Board
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    // find all horizontal matches given a position (x,y) in the Board
    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    // find horizontal and vertical matches at a position (x,y) in the Board
    List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }
        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    // find all matches given a list of GamePieces
    List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.m_xIndex, piece.m_yIndex, minLength)).ToList();
        }
        return matches;

    }

    // find all matches in the game Board
    List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    // turn off the temporary highlight
    void HighlightTileOff(int x, int y)
    {
        if (m_allTiles[x, y].m_tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    // temporary method to draw a highlight around a Tile
    void HighlightTileOn(int x, int y, Color col)
    {
        if (m_allTiles[x, y].m_tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = col;
        }
    }
    // highlight all matching tiles at position (x,y) in the Board
    void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);
        var combinedMatches = FindMatchesAt(x, y);
        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.m_xIndex, piece.m_yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    // highlight all matching tiles in the Board
    void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);

            }
        }
    }

    // highlight Tiles that correspond to a list of GamePieces
    void HighlightPieces(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.m_xIndex, piece.m_yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    // clear the GamePiece at position (x,y) in the Board
    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        //HighlightTileOff(x,y);
    }

    // clear the entire Board
    void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);

                if (m_particleManager != null)
                {
                    m_particleManager.ClearPieceFXAt(i, j);
                }
            }
        }
    }

    // clear a list of GamePieces (plus a potential sublist of GamePieces destroyed by bombs)
    void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                // clear the GamePiece
                ClearPieceAt(piece.m_xIndex, piece.m_yIndex);

                // add a score bonus if we clear four or more pieces
                int bonus = 0;
                if (gamePieces.Count >= 4)
                {
                    bonus = 20;
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ScorePoints(piece);

                    TimeBonus timeBonus = piece.GetComponent<TimeBonus>();

                    if (timeBonus != null)
                    {
                        GameManager.Instance.AddTime(timeBonus.m_bonusValue);
                        //                        Debug.Log("BOARD Adding time bonus from " + piece.name + " of " + timeBonus.bonusValue + "seconds");
                    }

                    GameManager.Instance.UpdateCollectionGoals(piece);
                }

                // play particle effects for pieces...
                if (m_particleManager != null)
                {
                    // ... cleared by bombs
                    if (bombedPieces.Contains(piece))
                    {
                        m_particleManager.BombFXAt(piece.m_xIndex, piece.m_yIndex);
                    }
                    // ... cleared normally
                    else
                    {
                        m_particleManager.ClearPieceFXAt(piece.m_xIndex, piece.m_yIndex);
                    }
                }
            }
        }
    }

    // damage a Breakable Tile
    void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = m_allTiles[x, y];

        if (tileToBreak != null && tileToBreak.m_tileType == TileType.Breakable)
        {
            // play appropriate particle effect
            if (m_particleManager != null)
            {
                m_particleManager.BreakTileFXAt(tileToBreak.m_breakableValue, x, y, 0);
            }

            tileToBreak.BreakTile();
        }
    }

    // break Tiles corresponding to a list of gamePieces
    void BreakTileAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                BreakTileAt(piece.m_xIndex, piece.m_yIndex);
            }
        }
    }

    // compresses a given column to remove any empty Tile spaces
    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        // running list of GamePieces that we need to move
        List<GamePiece> movingPieces = new List<GamePiece>();

        // loop from the bottom of the column
        for (int i = 0; i < height - 1; i++)
        {
            // if the current space is empty and not occupied by an Obstacle Tile...
            if (m_allGamePieces[column, i] == null && m_allTiles[column, i].m_tileType != TileType.Obstacle)
            {
                // ...loop from the space above it to the top of the column, to search for the next GamePiece
                for (int j = i + 1; j < height; j++)
                {
                    // if we find a GamePiece...
                    if (m_allGamePieces[column, j] != null)
                    {
                        // move the GamePiece downward to fill in the space and update the GamePiece array
                        m_allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        m_allGamePieces[column, i] = m_allGamePieces[column, j];
                        m_allGamePieces[column, i].SetCoord(column, i);

                        // add our piece to the list of pieces that we are moving
                        if (!movingPieces.Contains(m_allGamePieces[column, i]))
                        {
                            movingPieces.Add(m_allGamePieces[column, i]);
                        }


                        m_allGamePieces[column, j] = null;

                        // break out of the loop and stop searching 
                        break;
                    }
                }
            }
        }
        // return our list of GamePieces that are being moved
        return movingPieces;
    }

    // collapse all columns given a list of GamePieces
    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }

    // given a List of GamePieces, return a list of columns by index number
    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (!columns.Contains(piece.m_xIndex))
                {
                    columns.Add(piece.m_xIndex);
                }
            }
        }
        return columns;
    }

    // clear and refill the Board
    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    // coroutine to clear GamePieces and collapse empty spaces, then refill the Board
    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {

        // disable player input so we cannot swap pieces while the Board is collapsing/refilling
        m_playerInputEnabled = false;

        isRefilling = true;

        // create a new List of GamePieces, using our initial list as a starting point
        List<GamePiece> matches = gamePieces;

        // store a score multiplier for chain reactions
        m_scoreMultiplier = 0;
        do
        {
            //  increment our score multiplier by 1 for each subsequent recursive call of ClearAndCollapseRoutine
            m_scoreMultiplier++;

            // run the coroutine to clear the Board and collapse any columns to fill in the spaces
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));

            // pause one frame
            yield return null;

            // run the coroutine to refill the Board
            yield return StartCoroutine(RefillRoutine());

            // find any subsequent matches and repeat the process...
            matches = FindAllMatches();

            yield return new WaitForSeconds(0.2f);

        }
        // .. while our list of matches still has GamePieces in it
        while (matches.Count != 0);


        // deadlock check
        if (m_boardDeadlock.IsDeadlocked(m_allGamePieces, 3))
        {
            yield return new WaitForSeconds(1f);
            // ClearBoard();

            // shuffle the Board's normal pieces instead of Clearing out the whole Board
            yield return StartCoroutine(ShuffleBoardRoutine());

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(RefillRoutine());
        }


        // re-enable player input
        m_playerInputEnabled = true;

        isRefilling = false;

    }

    // coroutine to clear GamePieces from the Board and collapse any empty spaces
    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        // list of GamePieces that will be moved
        List<GamePiece> movingPieces = new List<GamePiece>();

        // list of GamePieces that form matches
        List<GamePiece> matches = new List<GamePiece>();

        yield return YieldInstructionCache.WaitForSeconds(0.2f);

        bool isFinished = false;

        while (!isFinished)
        {
            // check the original list for bombs and append any pieces affected by these bombs
            List<GamePiece> bombedPieces = new List<GamePiece>();
            List<GamePiece> savedBombedPieces = new List<GamePiece>();

            do {
                // combine that with our original list
                gamePieces = gamePieces.Union(bombedPieces).ToList();

                // repeat this check once to see if we hit any more bombs 
                bombedPieces = GetBombedPieces(gamePieces);
                savedBombedPieces = savedBombedPieces.Union(bombedPieces).ToList();

                gamePieces = gamePieces.Union(bombedPieces).ToList();

            } while (HasBombInside(gamePieces));


            // store what columns need to be collapsed
            List<int> columnsToCollapse = GetColumns(gamePieces);

            // clear the GamePieces, pass in the list of GamePieces affected by bombs as a separate list
            ClearPieceAt(gamePieces, savedBombedPieces);
            // break any tiles under the cleared GamePieces
            BreakTileAt(gamePieces);

            // activate any bombs in our clicked or target Tiles
            if (m_clickedTileBomb != null)
            {
                ActivateBomb(m_clickedTileBomb);
                m_clickedTileBomb = null;
            }

            if (m_targetTileBomb != null)
            {
                ActivateBomb(m_targetTileBomb);
                m_targetTileBomb = null;

            }

            // after a delay, collapse the columns to remove any empty spaces
            yield return new WaitForSeconds(0.25f);

            // collapse any columns with empty spaces and keep track of what pieces moved as a result
            movingPieces = CollapseColumn(columnsToCollapse);

            // wait while these pieces fill in the gaps
            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);


            // find any matches that form from collapsing...
            matches = FindMatchesAt(movingPieces);
            

            // if we didn't make any matches from the collapse, then we're done
            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }

            // otherwise, increase our score multiplier for the chair reaction... 
            else
            {
                m_scoreMultiplier++;

                // ...play a bonus sound for making a chain reaction...
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBonusSound();
                }

                // ...and run ClearAndCollapse again
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    // coroutine to refill the Board
    IEnumerator RefillRoutine()
    {
        FillBoard(fillYOffset, fillMoveTime);

        yield return null;

    }

    // checks if the GamePieces have reached their destination positions on collapse
    bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.m_yIndex > 0.001f)
                {
                    return false;
                }

                if (piece.transform.position.x - (float)piece.m_xIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // gets a list of GamePieces in a specified row
    List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            if (m_allGamePieces[i, row] != null)
            {
                gamePieces.Add(m_allGamePieces[i, row]);
            }
        }
        return gamePieces;
    }

    // gets a list of GamePieces in a specified column
    List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            if (m_allGamePieces[column, i] != null)
            {
                gamePieces.Add(m_allGamePieces[column, i]);
            }
        }
        return gamePieces;
    }

    // get all GamePieces adjacent to a position (x,y)
    List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    gamePieces.Add(m_allGamePieces[i, j]);
                }

            }
        }

        return gamePieces;
    }

    // given a list of GamePieces, returns a new List of GamePieces that would be destroyed by bombs from the original list
    List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {
        // list of GamePieces to clear
        List<GamePiece> allPiecesToClear = new List<GamePiece>();

        // loop through the original list of GamePieces
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                // list of GamePieces to be cleared by bombs
                List<GamePiece> piecesToClear = new List<GamePiece>();

                // check each GamePiece if it has a Bomb
                Bomb bomb = piece.GetComponent<Bomb>();

                // if so, get a list of GamePieces affected
                if (bomb != null)
                {
                    switch (bomb.m_bombType)
                    {
                        case BombType.Column:
                            piecesToClear = GetColumnPieces(bomb.m_xIndex);
                            break;
                        case BombType.Row:
                            piecesToClear = GetRowPieces(bomb.m_yIndex);
                            break;
                        case BombType.Adjacent:
                            piecesToClear = GetAdjacentPieces(bomb.m_xIndex, bomb.m_yIndex, 1);
                            break;
                        case BombType.Color:
                            /* */
                            break;
                    }
                    bomb.m_bombType = BombType.None;

                    // keep a running list of all GamePieces affected by bombs
                    allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();

                }
            }
        }
        // return a list of all GamePieces that would be affected by any bombs from the original list
        return allPiecesToClear;
    }

    // check if List of matching GamePieces forms an L shaped match
    bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        // loop through all of the pieces
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                // if this is the very first piece we are checking, save its x and y index
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.m_xIndex;
                    yStart = piece.m_yIndex;
                    continue;
                }

                // otherwise, see if GamePiece is in line horizontally with the first piece
                if (piece.m_xIndex != xStart && piece.m_yIndex == yStart)
                {
                    horizontal = true;
                }

                // check if are in line vertically with the first piece
                if (piece.m_xIndex == xStart && piece.m_yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }

        // return true only if pieces align both horizontally and vertically with first piece
        return (horizontal && vertical);

    }

    // drops a Bomb at a position (x,y) in the Board, given a list of matching GamePieces
    GameObject DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> gamePieces)
    {

        GameObject bomb = null;
        MatchValue matchValue = MatchValue.None;

        if (gamePieces != null)
        {
            matchValue = FindMatchValue(gamePieces);
        }

        // check if the GamePieces are four or more in a row
        if (gamePieces.Count >= 5 && matchValue != MatchValue.None)
        {
            // check if we form a corner match and create an adjacent bomb
            if (IsCornerMatch(gamePieces))
            {
                GameObject adjacentBomb = FindGamePieceByMatchValue(adjacentBombPrefabs, matchValue);

                if (adjacentBomb != null)
                {
                    bomb = MakeBomb(adjacentBomb, x, y);
                }
            }
            else
            {
                // if have five or more in a row, form a color bomb - note we probably should swap this upward to 
                // give it priority over an adjacent bomb

                if (colorBombPrefab != null)
                {
                    bomb = MakeBomb(colorBombPrefab, x, y);

                }
            }
        }
        else if (gamePieces.Count == 4 && matchValue != MatchValue.None)
        {
            // otherwise, drop a row bomb if we are swiping sideways
            if (swapDirection.x != 0)
            {
                GameObject columnBomb = FindGamePieceByMatchValue(columnBombPrefabs, matchValue);
                // or drop a vertical bomb if we are swiping upwards
                if (columnBomb != null)
                {
                    bomb = MakeBomb(columnBomb, x, y);
                }

            }
            else
            {
                GameObject rowBomb = FindGamePieceByMatchValue(rowBombPrefabs, matchValue);
                if (rowBomb != null)
                {
                    bomb = MakeBomb(rowBomb, x, y);
                }
            }
        }
        // return the Bomb object
        return bomb;
    }

    // puts the bomb into the game Board and treats it as a normal GamePiece
    void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;


        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

    // find all GamePieces on the Board with a certain MatchValue
    List<GamePiece> FindAllMatchValue(MatchValue mValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allGamePieces[i, j] != null)
                {
                    if (m_allGamePieces[i, j].m_matchValue == mValue)
                    {
                        foundPieces.Add(m_allGamePieces[i, j]);
                    }
                }
            }
        }
        return foundPieces;
    }

    // return if the Bomb is a Color Bomb
    bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();

        if (bomb != null)
        {
            return (bomb.m_bombType == BombType.Color);
        }
        return false;
    }



    // given a list of GamePieces, return the first valid MatchValue found
    MatchValue FindMatchValue(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                return piece.m_matchValue;
            }
        }

        return MatchValue.None;
    }

    // given an array of prefabs, find one whose GamePiece component has a given matchValue
    GameObject FindGamePieceByMatchValue(GameObject[] gamePiecePrefabs, MatchValue matchValue)
    {
        if (matchValue == MatchValue.None)
        {
            return null;
        }

        foreach (GameObject go in gamePiecePrefabs)
        {
            GamePiece piece = go.GetComponent<GamePiece>();

            if (piece != null)
            {
                if (piece.m_matchValue == matchValue)
                {
                    return go;
                }
            }
        }

        return null;

    }

    public void TestDeadlock()
    {
        m_boardDeadlock.IsDeadlocked(m_allGamePieces, 3);
    }

    // invoke the ShuffleBoardRoutine (called by a button for testing)
    public void ShuffleBoard()
    {
        // only shuffle if the Board permits user input
        if (m_playerInputEnabled)
        {
            StartCoroutine(ShuffleBoardRoutine());
        }

    }

    // shuffle non-bomb and non-collectible GamePieces
    IEnumerator ShuffleBoardRoutine()
    {
        // get a list of all the GamePieces
        List<GamePiece> allPieces = new List<GamePiece>();
        foreach (GamePiece piece in m_allGamePieces)
        {
            allPieces.Add(piece);
        }

        // wait for any GamePieces that have not settled into place
        while (!IsCollapsed(allPieces))
        {
            yield return null;
        }

        // remove any normalPieces from m_allGamePieces and store them in a List
        List<GamePiece> normalPieces = m_boardShuffler.RemoveNormalPieces(m_allGamePieces);

        // shuffle the list of normal pieces
        m_boardShuffler.ShuffleList(normalPieces);

        // use the shuffled list to fill the Board
        FillBoardFromList(normalPieces);

        // move the pieces to their correct onscreen positions
        m_boardShuffler.MovePieces(m_allGamePieces, swapTime);

        // in the event some matches form, clear and refill the Board
        List<GamePiece> matches = FindAllMatches();
        StartCoroutine(ClearAndRefillBoardRoutine(matches));

    }


}
