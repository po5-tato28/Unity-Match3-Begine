using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will stroe normal(non-special) pieces and shuffle
// Fisher-Yates_shuffle:: https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

public class BoardShuffler : MonoBehaviour
{
    public List<GamePiece> RemoveNormalPieces(GamePiece[,] _allPieces) {

        List<GamePiece> normalPieces = new List<GamePiece>();

        int width = _allPieces.GetLength(0);
        int height = _allPieces.GetLength(1);

        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {

                if (_allPieces[i, j] != null) {
                    Bomb bomb = _allPieces[i, j].GetComponent<Bomb>();
                    normalPieces.Add(_allPieces[i, j]);
                    _allPieces[i, j] = null;

                }// if1

            }// for2
        }// for1

        return normalPieces;
    }

    public void ShuffleList(List<GamePiece> _piecesToShuffle) {

        int maxCount = _piecesToShuffle.Count;

        for(int i = 0; i < maxCount -1; i++) {

            int r = Random.Range(i, maxCount);
            if(r == i) {
                continue;
            }

            GamePiece temp = _piecesToShuffle[r];
            _piecesToShuffle[r] = _piecesToShuffle[i];
            _piecesToShuffle[i] = temp;
        }
    }

    public void MovePieces(GamePiece[,] _allPieces, float _swapTime = 0.5f) {

        int width = _allPieces.GetLength(0);
        int height = _allPieces.GetLength(1);

        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {

                if(_allPieces[i, j] != null) {
                    _allPieces[i, j].Move(i, j, _swapTime); 
                } // if

            }// for2
        }// for1
    }
}
